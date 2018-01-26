using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MorphAI_Delinquent {

	/// <summary>
	/// Primary processing module for the Delinquent AI. Calculates and 
	/// selects AI moves during gameplay.
	/// </summary> 
	/// <para>
	/// Author: Robert Thompson
	/// Date: March 14, 2017
	/// </para>
	class DelinquentBrain {

		public static Benchmarker bench = new Benchmarker();

		private double pieceValueWeight;
		private double boardAdvanceWeight;
		private double threatWeight;
		private double mobilityWeight;

		private int maxDepth;
		private int currentDepth;
		private bool timeExpired;
		private Stopwatch turnTimer;
		private double maxTurnTime;

		private Game.PIECES[,] board;
		private Dictionary<long, Transposition> transTable;

		private Stack<Removal> removals;

		public DelinquentBrain() {
			pieceValueWeight = 0.45;
			boardAdvanceWeight = 0.15;
			threatWeight = 0.25;
			mobilityWeight = 0.15;
			maxDepth = 4;
			timeExpired = false;
			turnTimer = new Stopwatch();
			maxTurnTime = 4990.0;
			currentDepth = 0;
			board = new Game.PIECES[8, 6];
			removals = new Stack<Removal>();
			transTable = new Dictionary<long, Transposition>();
		}//end of constructor

		public int[] GetComputerMove() {
			Move move = null;
			maxDepth = 4;
			timeExpired = false;
			turnTimer.Restart();
			do {
				Move thisMove = Minimax();
				maxDepth++;
				if(thisMove != null) {
					move = thisMove;
				}
			}
			while(!timeExpired);
//			Console.WriteLine($"MaxDepth Reached = {maxDepth-1}");
			return move.ToArray();
		}//end of GetComputerMove method

		public int[] GetRANDComputerMove() {
			List<Move> moves = Delinquent.GetGame().MoveGenerator(Delinquent.GetGame().GetComputerPlayer());
			int number = moves.Count();
			Random rng = new Random();
			int randIndex = rng.Next(0, number);
			Move move = moves.ElementAt(randIndex);
			return move.ToArray();
		}//end of GetRANDComputerMove method

		private Move Minimax() {
			Move returnValue = null;
			int bestScore = int.MinValue;
			List<Move> moves = Delinquent.GetGame().MoveGenerator(Delinquent.GetGame().GetComputerPlayer());
			foreach(Move move in moves) {
				if(timeExpired) {
					break;
				}
				currentDepth = 1;
				Delinquent.GetGame().TryMove(move);
				int moveScore = Min(bestScore);
				if(moveScore > bestScore) {
					returnValue = move;
					bestScore = moveScore;
				}
				Delinquent.GetGame().UndoMove(move);
			}
			if(timeExpired) {
				returnValue = null;
			}
			return returnValue;
		}//end of Minimax method

		private int Max(int min) {
			int returnValue = int.MinValue;
			if(turnTimer.Elapsed.TotalMilliseconds >= maxTurnTime) {
				timeExpired = true;
				return returnValue;
			}
			currentDepth++;
			Game.PLAYERS computer = Delinquent.GetGame().GetComputerPlayer();
			List<Move> moves = Delinquent.GetGame().MoveGenerator(Delinquent.GetGame().GetComputerPlayer());
			if(computer == Game.PLAYERS.P1) {
				if(moves.Count == 0 || Delinquent.GetGame().PeekRemoval().GetPiece() == Game.PIECES.P1_KING) {
					returnValue = -999999999;
				}
			}
			else {
				if(moves.Count == 0 || Delinquent.GetGame().PeekRemoval().GetPiece() == Game.PIECES.P2_KING) {
					returnValue = -999999999;
				}
			}
			if(returnValue != -999999999) {
				if(currentDepth == maxDepth) {
					returnValue = Eval(moves, true);
				}
				else {
					foreach(Move move in moves) {
						if(timeExpired) {
							break;
						}
						Delinquent.GetGame().TryMove(move);
						int moveScore = Min(returnValue);
						if(moveScore > returnValue) {
							returnValue = moveScore;
						}
						Delinquent.GetGame().UndoMove(move);
						if(returnValue > min) {
							break;
						}
					}
				}
			}
			currentDepth--;
			return returnValue;
		}//end of Max method

		private int Min(int max) {
			int returnValue = int.MaxValue;
			if(turnTimer.Elapsed.TotalMilliseconds >= maxTurnTime) {
				timeExpired = true;
				return returnValue;
			}
			currentDepth++;
			Game.PLAYERS human = Delinquent.GetGame().GetHumanPlayer();
			List<Move> moves = Delinquent.GetGame().MoveGenerator(Delinquent.GetGame().GetHumanPlayer());
			if(human == Game.PLAYERS.P1) {
				if(moves.Count == 0 || Delinquent.GetGame().PeekRemoval().GetPiece() == Game.PIECES.P1_KING) {
					returnValue = 999999999;
				}
			}
			else {
				if(moves.Count == 0 || Delinquent.GetGame().PeekRemoval().GetPiece() == Game.PIECES.P2_KING) {
					returnValue = 999999999;
				}
			}
			if(returnValue != 999999999) {
				if(currentDepth == maxDepth) {
					returnValue = Eval(moves, false);
				}
				else {
					foreach(Move move in moves) {
						if(timeExpired) {
							break;
						}
						Delinquent.GetGame().TryMove(move);
						int moveScore = Max(returnValue);
						if(moveScore < returnValue) {
							returnValue = moveScore;
						}
						Delinquent.GetGame().UndoMove(move);
						if(returnValue < max) {
							break;
						}
					}
				}
			}
			currentDepth--;
			return returnValue;
		}//end of Min method

		private int Eval(List<Move> moves, bool forComputer) {
			int returnValue = 0;
			bool useTrans = false;
			long transKey = Delinquent.GetGame().GetCurrentHash();
			if(transTable.ContainsKey(transKey)) {
				Transposition trans = transTable[transKey];
				if(trans.Ply() <= currentDepth) {
					returnValue = trans.Score();
					useTrans = true;
				}
			}
			if(!useTrans) {
				Game.PLAYERS human = Delinquent.GetGame().GetHumanPlayer();
				Game.PLAYERS computer = Delinquent.GetGame().GetComputerPlayer();
				List<Move> computerMoves;
				List<Move> humanMoves;
				if(forComputer) {
					computerMoves = moves;
					humanMoves = Delinquent.GetGame().MoveGenerator(human);
				}
				else {
					humanMoves = moves;
					computerMoves = Delinquent.GetGame().MoveGenerator(computer);
				}
				//piece value + board advance + mobility + piece threat
				if(human == Game.PLAYERS.P1) {
					int p1Score = EvalP1Score(humanMoves);
					int p2Score = EvalP2Score(computerMoves);
					returnValue = p2Score - p1Score;
				}
				else {
					int p1Score = EvalP1Score(computerMoves);
					int p2Score = EvalP2Score(humanMoves);
					returnValue = p1Score - p2Score;
				}
				transTable[transKey] = new Transposition(currentDepth, returnValue);
			}
			return returnValue;
		}//end of Eval method

		private int EvalP1Score(List<Move> p1Moves) {
			int pieceValue = 0;
			int boardAdvance = 0;
			int threat = 0;
			Pos currentPos = null;
			List<Pos> pieces = new List<Pos>();
			List<Target> captures = new List<Target>();
			foreach(Move move in p1Moves) {
				if(currentPos == null) {
					currentPos = new Pos(move);
					pieces.Add(currentPos);
				}
				else if(!currentPos.Equals(move)) {
					currentPos = new Pos(move);
					pieces.Add(currentPos);
				}
				Target tar = new Target(move);
				if(tar.piece != Game.PIECES.NONE) {
					captures.Add(tar);
				}
			}
			foreach(Pos pos in pieces) {
				pieceValue += PieceValueP1(pos.piece);
				if(pos.piece == Game.PIECES.P1_KING) {
					boardAdvance += (4 - pos.col);
				}
				else if(pos.piece == Game.PIECES.P1_ROOK) {
					boardAdvance += (8 - pos.row);
				}
				else {
					boardAdvance += (6 - pos.row);
				}
			}
			foreach(Target tar in captures) {
				switch(tar.piece) {
				case Game.PIECES.NONE:
					break;
				case Game.PIECES.P2_KING:
					threat += 10;
					break;
				case Game.PIECES.P2_BISHOP:
					threat += 5;
					break;
				case Game.PIECES.P2_ROOK:
					threat += 5;
					break;
				case Game.PIECES.P2_KNIGHT:
					threat += 5;
					break;
				case Game.PIECES.P2_PAWN:
					threat += 2;
					break;
				}
			}
			int mobility = (int)(Math.Ceiling((float)p1Moves.Count / pieces.Count));
			//piece value + board advance + mobility + piece threat
			return (int)(Math.Round((pieceValueWeight * pieceValue) + (boardAdvanceWeight * boardAdvance) + (mobilityWeight * mobility) + (threatWeight * threat)));
		}//end of EvalP1Score method

		private int EvalP2Score(List<Move> p2Moves) {
			int pieceValue = 0;
			int boardAdvance = 0;
			int threat = 0;
			Pos currentPos = null;
			List<Pos> pieces = new List<Pos>();
			List<Target> captures = new List<Target>();
			foreach(Move move in p2Moves) {
				if(currentPos == null) {
					currentPos = new Pos(move);
					pieces.Add(currentPos);
				}
				else if(!currentPos.Equals(move)) {
					currentPos = new Pos(move);
					pieces.Add(currentPos);
				}
				Target tar = new Target(move);
				if(tar.piece != Game.PIECES.NONE) {
					captures.Add(tar);
				}
			}
			foreach(Pos pos in pieces) {
				pieceValue += PieceValueP1(pos.piece);
				if(pos.piece == Game.PIECES.P2_KING) {
					boardAdvance += (4 - pos.col);
				}
				else if(pos.piece == Game.PIECES.P2_ROOK) {
					boardAdvance += (8 - pos.row);
				}
				else {
					boardAdvance += (6 - pos.row);
				}
			}
			foreach(Target tar in captures) {
				switch(tar.piece) {
				case Game.PIECES.NONE:
					break;
				case Game.PIECES.P1_KING:
					threat += 10;
					break;
				case Game.PIECES.P1_BISHOP:
					threat += 5;
					break;
				case Game.PIECES.P1_ROOK:
					threat += 5;
					break;
				case Game.PIECES.P1_KNIGHT:
					threat += 5;
					break;
				case Game.PIECES.P1_PAWN:
					threat += 2;
					break;
				}
			}
			int mobility = (int)(Math.Ceiling((float)p2Moves.Count / pieces.Count));
			//piece value + board advance + mobility + piece threat
			return (int)(Math.Round((pieceValueWeight * pieceValue) + (boardAdvanceWeight * boardAdvance) + (mobilityWeight * mobility) + (threatWeight * threat)));
		}//end of EvalP2Score method

		private int PieceValueP1(Game.PIECES piece) {
			int returnValue = 0;
			switch(piece) {
			case Game.PIECES.P1_ROOK:
				returnValue = 7;
				break;
			case Game.PIECES.P1_BISHOP:
				returnValue = 4;
				break;
			case Game.PIECES.P1_KNIGHT:
				returnValue = 5;
				break;
			case Game.PIECES.P1_PAWN:
				returnValue = 1;
				break;
			}
			return returnValue;
		}//end of PieceValue method

		private int PieceValueP2(Game.PIECES piece) {
			int returnValue = 0;
			switch(piece) {
			case Game.PIECES.P2_ROOK:
				returnValue = 7;
				break;
			case Game.PIECES.P2_BISHOP:
				returnValue = 4;
				break;
			case Game.PIECES.P2_KNIGHT:
				returnValue = 5;
				break;
			case Game.PIECES.P2_PAWN:
				returnValue = 1;
				break;
			}
			return returnValue;
		}//end of PieceValue method
		
		private class Pos {
			public int row;
			public int col;
			public Game.PIECES piece;

			public Pos(int row, int col, Game.PIECES piece) {
				this.row = row;
				this.col = col;
				this.piece = piece;
			}//end of (int,int) constructor

			public Pos(Move move) {
				this.row = move.GetFromRow();
				this.col = move.GetFromCol();
				this.piece = Delinquent.GetGame().PieceAt(row, col);
			}//end of (Move) constructor

			public bool Equals(Move move) {
				bool returnValue = false;
				if(row == move.GetFromRow() && col == move.GetFromCol()){
					returnValue = true;
				}
				return returnValue;
			}//end of Equals(Move) method

		}//end of Pos class

		private class Target {
			public int row;
			public int col;
			public Game.PIECES piece;

			public Target(int row, int col, Game.PIECES piece) {
				this.row = row;
				this.col = col;
				this.piece = piece;
			}//end of (int,int) constructor

			public Target(Move move) {
				this.row = move.GetToRow();
				this.col = move.GetToCol();
				piece = Delinquent.GetGame().PieceAt(row, col);
			}//end of (Move) constructor

			public bool Equals(Move move) {
				bool returnValue = false;
				if(row == move.GetToRow() && col == move.GetToCol()) {
					returnValue = true;
				}
				return returnValue;
			}//end of Equals(Move) method

			public bool Equals(Target tar) {
				bool returnValue = false;
				if(this.row == tar.row && this.col == tar.col && this.piece == tar.piece) {
					returnValue = true;
				}
				return returnValue;
			}//end of Equals(Target) method

		}//end of Pos class

		private class Transposition {
			private int ply;
			private int score;

			public Transposition(int ply, int score) {
				this.ply = ply;
				this.score = score;
			}//end of constructor

			public int Ply() {
				return ply;
			}//end of Ply method

			public int Score() {
				return score;
			}//end of Score method
		}//end of Transposition class

		public class Benchmarker {

			private List<long> evalTicks;
			private List<long> moveGenTicks;
			private List<long> minimaxTicks;
			private List<double> evalTimes;
			private List<double> moveGenTimes;
			private List<double> minimaxTimes;
			private List<double> evalTickAves;
			private List<double> moveGenTickAves;
			private List<double> minimaxTickAves;
			private List<double> evalTimeAves;
			private List<double> moveGenTimeAves;
			private List<double> minimaxTimeAves;

			public Benchmarker() {
				evalTicks = new List<long>();
				moveGenTicks = new List<long>();
				minimaxTicks = new List<long>();
				evalTimes = new List<double>();
				moveGenTimes = new List<double>();
				minimaxTimes = new List<double>();
				evalTickAves = new List<double>();
				moveGenTickAves = new List<double>();
				minimaxTickAves = new List<double>();
				evalTimeAves = new List<double>();
				moveGenTimeAves = new List<double>();
				minimaxTimeAves = new List<double>();
			}//end of constructor

			public void AddEval(long ticks, double time) {
				evalTicks.Add(ticks);
				evalTimes.Add(time);
			}//end of AddEval method

			public void AddMoveGen(long ticks, double time) {
				moveGenTicks.Add(ticks);
				moveGenTimes.Add(time);
			}//end of AddMoveGen method

			public void AddMinimax(long ticks, double time) {
				minimaxTicks.Add(ticks);
				minimaxTimes.Add(time);
			}//end of AddMinimax method

			public void Clear() {
				evalTicks.Clear();
				moveGenTicks.Clear();
				minimaxTicks.Clear();
				evalTimes.Clear();
				moveGenTimes.Clear();
				minimaxTimes.Clear();
			}//end of Clear method

			public void Reset() {
				evalTicks.Clear();
				moveGenTicks.Clear();
				minimaxTicks.Clear();
				evalTimes.Clear();
				moveGenTimes.Clear();
				minimaxTimes.Clear();
				evalTickAves.Clear();
				moveGenTickAves.Clear();
				minimaxTickAves.Clear();
				evalTimeAves.Clear();
				moveGenTimeAves.Clear();
				minimaxTimeAves.Clear();
			}//end of Reset method

			public void PrintLap() {
				long evalSum = 0;
				foreach(long eval in evalTicks) {
					evalSum += eval;
				}
				double aveEval = (double)evalSum / evalTicks.Count;
				evalTickAves.Add(aveEval);
				double eTimeSum = 0;
				foreach(double eTime in evalTimes) {
					eTimeSum += eTime;
				}
				double aveETime = eTimeSum / evalTimes.Count;
				evalTimeAves.Add(aveETime);
				Console.WriteLine($"Eval calls: {evalTicks.Count} Ave Ticks: {aveEval} Ave Time: {aveETime}");
				long genSum = 0;
				foreach(long gen in moveGenTicks) {
					genSum += gen;
				}
				double aveGen = (double)genSum / moveGenTicks.Count;
				moveGenTickAves.Add(aveGen);
				double gTimeSum = 0;
				foreach(double gTime in moveGenTimes) {
					gTimeSum += gTime;
				}
				double aveGTime = gTimeSum / moveGenTimes.Count;
				moveGenTimeAves.Add(aveGTime);
				Console.WriteLine($"MoveGen calls: {moveGenTicks.Count} Ave Ticks: {aveGen} Ave Time: {aveGTime}");
				long minimaxSum = 0;
				foreach(long mm in minimaxTicks) {
					minimaxSum += mm;
				}
				double aveMM = (double)minimaxSum / minimaxTicks.Count;
				minimaxTickAves.Add(aveMM);
				double mTimeSum = 0;
				foreach(double mTime in minimaxTimes) {
					mTimeSum += mTime;
				}
				double aveMTime = mTimeSum / minimaxTimes.Count;
				minimaxTimeAves.Add(aveMTime);
				Console.WriteLine($"Minimax calls: {minimaxTicks.Count} Ave Ticks: {aveMM} Ave Time: {aveMTime}");
				Console.WriteLine();
			}//end of PrintLap method

			public void PrintRun() {
				double evalSum = 0;
				foreach(double eval in evalTickAves) {
					evalSum += eval;
				}
				double aveEval = evalSum / evalTickAves.Count;
				double eTimeSum = 0;
				foreach(double eTime in evalTimeAves) {
					eTimeSum += eTime;
				}
				double aveETime = eTimeSum / evalTimeAves.Count;
				Console.WriteLine($"Eval Ave Count: {evalTickAves.Count} Overall Ave: {aveEval}/{aveETime}");
				double genSum = 0;
				foreach(double gen in moveGenTickAves) {
					genSum += gen;
				}
				double aveGen = genSum / moveGenTickAves.Count;
				double gTimeSum = 0;
				foreach(double gTime in moveGenTimeAves) {
					gTimeSum += gTime;
				}
				double aveGTime = gTimeSum / moveGenTimeAves.Count;
				Console.WriteLine($"MoveGen calls: {moveGenTickAves.Count} Overall Ave: {aveGen}/{aveGTime}");
				double minimaxSum = 0;
				foreach(double mm in minimaxTickAves) {
					minimaxSum += mm;
				}
				double aveMM = minimaxSum / minimaxTickAves.Count;
				double mTimeSum = 0;
				foreach(double mTime in minimaxTimeAves) {
					mTimeSum += mTime;
				}
				double aveMTime = mTimeSum / minimaxTimeAves.Count;
				Console.WriteLine($"Minimax calls: {minimaxTickAves.Count} Overall Ave: {aveMM}/{aveMTime}");
				Console.WriteLine();
			}//end of PrintRun method

		}//end of Benchmarker class

	}//end of DelinquentBrain class
	
}//end of Thompsor namespace