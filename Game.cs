using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MorphAI_Delinquent {

	/// <summary>
	/// 
	/// </summary> 
	/// <para>
	/// Author: Robert Thompson
	/// Date: March 14, 2017
	/// </para>
	class Game {

		public enum PIECES {
			NONE,
			P1_KING,
			P1_KNIGHT,
			P1_BISHOP,
			P1_ROOK,
			P1_PAWN,
			P2_KING,
			P2_KNIGHT,
			P2_BISHOP,
			P2_ROOK,
			P2_PAWN
		}//end of PIECES enum

		public enum PLAYERS {
			P1,
			P2,
			NEITHER
		}//end of PLAYERS enum
		
		private bool gameInProgress;

		private PLAYERS winner;

		private PLAYERS computer;
		private PLAYERS human;
		private ConsoleColor computerColor;
		private ConsoleColor humanColor;

		private PIECES[,] board;
		private long[,] zBoard;
		private long currentHash;

		private Stack<Removal> removals;

		public Game() {
			gameInProgress = false;
			winner = PLAYERS.NEITHER;
			computer = PLAYERS.P1;
			human = PLAYERS.P2;
			computerColor = ConsoleColor.DarkGreen;
			humanColor = ConsoleColor.White;
			board = new PIECES[8, 6];
			zBoard = new long[48, 10];
			currentHash = 0;
			removals = new Stack<Removal>();
		}//end of constructor

		public void InitBoard() {
			//first row
			board[0, 0] = PIECES.NONE;
			board[0, 1] = PIECES.P1_KING;
			board[0, 2] = PIECES.NONE;
			board[0, 3] = PIECES.NONE;
			board[0, 4] = PIECES.NONE;
			board[0, 5] = PIECES.NONE;
			//second row
			board[1, 0] = PIECES.P1_KNIGHT;
			board[1, 1] = PIECES.P1_BISHOP;
			board[1, 2] = PIECES.P1_ROOK;
			board[1, 3] = PIECES.P1_ROOK;
			board[1, 4] = PIECES.P1_BISHOP;
			board[1, 5] = PIECES.P1_KNIGHT;
			//third row
			board[2, 0] = PIECES.NONE;
			board[2, 1] = PIECES.NONE;
			board[2, 2] = PIECES.P1_PAWN;
			board[2, 3] = PIECES.P1_PAWN;
			board[2, 4] = PIECES.NONE;
			board[2, 5] = PIECES.NONE;
			//fourth row
			board[3, 0] = PIECES.NONE;
			board[3, 1] = PIECES.NONE;
			board[3, 2] = PIECES.NONE;
			board[3, 3] = PIECES.NONE;
			board[3, 4] = PIECES.NONE;
			board[3, 5] = PIECES.NONE;
			//fifth row
			board[4, 0] = PIECES.NONE;
			board[4, 1] = PIECES.NONE;
			board[4, 2] = PIECES.NONE;
			board[4, 3] = PIECES.NONE;
			board[4, 4] = PIECES.NONE;
			board[4, 5] = PIECES.NONE;
			//sixth row
			board[5, 0] = PIECES.NONE;
			board[5, 1] = PIECES.NONE;
			board[5, 2] = PIECES.P2_PAWN;
			board[5, 3] = PIECES.P2_PAWN;
			board[5, 4] = PIECES.NONE;
			board[5, 5] = PIECES.NONE;
			//seventh row
			board[6, 0] = PIECES.P2_KNIGHT;
			board[6, 1] = PIECES.P2_BISHOP;
			board[6, 2] = PIECES.P2_ROOK;
			board[6, 3] = PIECES.P2_ROOK;
			board[6, 4] = PIECES.P2_BISHOP;
			board[6, 5] = PIECES.P2_KNIGHT;
			//eighth row
			board[7, 0] = PIECES.NONE;
			board[7, 1] = PIECES.NONE;
			board[7, 2] = PIECES.NONE;
			board[7, 3] = PIECES.NONE;
			board[7, 4] = PIECES.P2_KING;
			board[7, 5] = PIECES.NONE;
			initZBoard();
		}//end of InitBoardAITop method

		private void initZBoard() {
			Random rng = new Random();
			for(int i = 0; i < 48; i++) {
				for(int j = 0; j < 10; j++) {
					byte[] buf = new byte[8];
					rng.NextBytes(buf);
					zBoard[i,j] = BitConverter.ToInt64(buf, 0);
				}
			}
			currentHash = 0;
			for(int x = 0; x < 8; x++) {
				for(int y = 0; y < 6; y++) {
					if(board[x, y] != PIECES.NONE) {
						currentHash = currentHash ^ zBoard[(5 * x) + y, (int)board[x, y] - 1];
					}
				}
			}
		}//end of initZBoard method

		public void StartGame() {
			gameInProgress = true;
		}//end of StartGame method

		public bool IsMoveLegal(int[] move, PLAYERS player) {
			bool returnValue = false;
			Move checkMove = new Move(move);
			List<Move> legalMoves = MoveGenerator(player);
//			Console.WriteLine("Moves Generated:");
			foreach(Move m in legalMoves) {
				//DEBUG
//				Console.WriteLine($"\t{m.ToString()}");
				//END DEBUG				
				if(checkMove == m) {
					returnValue = true;
				}
			}
			return returnValue;
		}//end of IsMoveLegal method

		public void EnterMove(int[] move) {
			Move newMove = new Move(move);
			Console.Write($" moves {GameInterface.map_board_location(move[0],move[1])}{GameInterface.map_board_location(move[2], move[3])} ({GameInterface.map_translate_location(move[0], move[1])}{GameInterface.map_translate_location(move[2], move[3])})");
			if(OnBoard(newMove.GetFromRow(), newMove.GetFromCol()) && OnBoard(newMove.GetFromRow(), newMove.GetFromCol())) {
				PIECES piece = board[newMove.GetFromRow(), newMove.GetFromCol()];
				currentHash = currentHash ^ zBoard[(5 * newMove.GetFromRow()) + newMove.GetFromCol(), (int)piece - 1];
				Removal removal = new Removal(newMove.GetToRow(), newMove.GetToCol(), board[newMove.GetToRow(), newMove.GetToCol()]);
				removals.Push(removal);
				if(board[newMove.GetToRow(), newMove.GetToCol()] != PIECES.NONE) {
					currentHash = currentHash ^ zBoard[(5 * newMove.GetToRow()) + newMove.GetToCol(), (int)board[newMove.GetToRow(), newMove.GetToCol()] - 1];
				}
				if(piece == PIECES.P1_BISHOP) {
					board[newMove.GetToRow(), newMove.GetToCol()] = PIECES.P1_KNIGHT;
					currentHash = currentHash ^ zBoard[(5 * newMove.GetToRow()) + newMove.GetToCol(), (int)PIECES.P1_KNIGHT - 1];
				}
				else if(piece == PIECES.P2_BISHOP) {
					board[newMove.GetToRow(), newMove.GetToCol()] = PIECES.P2_KNIGHT;
					currentHash = currentHash ^ zBoard[(5 * newMove.GetToRow()) + newMove.GetToCol(), (int)PIECES.P2_KNIGHT - 1];
				}
				else if(piece == PIECES.P1_ROOK) {
					board[newMove.GetToRow(), newMove.GetToCol()] = PIECES.P1_BISHOP;
					currentHash = currentHash ^ zBoard[(5 * newMove.GetToRow()) + newMove.GetToCol(), (int)PIECES.P1_BISHOP - 1];
				}
				else if(piece == PIECES.P2_ROOK) {
					board[newMove.GetToRow(), newMove.GetToCol()] = PIECES.P2_BISHOP;
					currentHash = currentHash ^ zBoard[(5 * newMove.GetToRow()) + newMove.GetToCol(), (int)PIECES.P2_BISHOP - 1];
				}
				else if(piece == PIECES.P1_KNIGHT) {
					board[newMove.GetToRow(), newMove.GetToCol()] = PIECES.P1_ROOK;
					currentHash = currentHash ^ zBoard[(5 * newMove.GetToRow()) + newMove.GetToCol(), (int)PIECES.P1_ROOK - 1];
				}
				else if(piece == PIECES.P2_KNIGHT) {
					board[newMove.GetToRow(), newMove.GetToCol()] = PIECES.P2_ROOK;
					currentHash = currentHash ^ zBoard[(5 * newMove.GetToRow()) + newMove.GetToCol(), (int)PIECES.P2_ROOK - 1];
				}
				else {
					board[newMove.GetToRow(), newMove.GetToCol()] = piece;
					currentHash = currentHash ^ zBoard[(5 * newMove.GetToRow()) + newMove.GetToCol(), (int)piece - 1];
				}
				board[newMove.GetFromRow(), newMove.GetFromCol()] = PIECES.NONE;
			}
		}//end of EnterMove method

		public void TryMove(Move move) {
			if(OnBoard(move.GetFromRow(), move.GetFromCol()) && OnBoard(move.GetFromRow(), move.GetFromCol())) {
				PIECES piece = board[move.GetFromRow(), move.GetFromCol()];
				currentHash = currentHash ^ zBoard[(5 * move.GetFromRow()) + move.GetFromCol(), (int)piece - 1];
				Removal removal = new Removal(move.GetToRow(), move.GetToCol(), board[move.GetToRow(), move.GetToCol()]);
				removals.Push(removal);
				if(board[move.GetToRow(), move.GetToCol()] != PIECES.NONE) {
					currentHash = currentHash ^ zBoard[(5 * move.GetToRow()) + move.GetToCol(), (int)board[move.GetToRow(), move.GetToCol()] - 1];
				}
				if(piece == PIECES.P1_BISHOP) {
					board[move.GetToRow(), move.GetToCol()] = PIECES.P1_KNIGHT;
					currentHash = currentHash ^ zBoard[(5 * move.GetToRow()) + move.GetToCol(), (int)PIECES.P1_KNIGHT - 1];
				}
				else if(piece == PIECES.P2_BISHOP) {
					board[move.GetToRow(), move.GetToCol()] = PIECES.P2_KNIGHT;
					currentHash = currentHash ^ zBoard[(5 * move.GetToRow()) + move.GetToCol(), (int)PIECES.P2_KNIGHT - 1];
				}
				else if(piece == PIECES.P1_ROOK) {
					board[move.GetToRow(), move.GetToCol()] = PIECES.P1_BISHOP;
					currentHash = currentHash ^ zBoard[(5 * move.GetToRow()) + move.GetToCol(), (int)PIECES.P1_BISHOP - 1];
				}
				else if(piece == PIECES.P2_ROOK) {
					board[move.GetToRow(), move.GetToCol()] = PIECES.P2_BISHOP;
					currentHash = currentHash ^ zBoard[(5 * move.GetToRow()) + move.GetToCol(), (int)PIECES.P2_BISHOP - 1];
				}
				else if(piece == PIECES.P1_KNIGHT) {
					board[move.GetToRow(), move.GetToCol()] = PIECES.P1_ROOK;
					currentHash = currentHash ^ zBoard[(5 * move.GetToRow()) + move.GetToCol(), (int)PIECES.P1_ROOK - 1];
				}
				else if(piece == PIECES.P2_KNIGHT) {
					board[move.GetToRow(), move.GetToCol()] = PIECES.P2_ROOK;
					currentHash = currentHash ^ zBoard[(5 * move.GetToRow()) + move.GetToCol(), (int)PIECES.P2_ROOK - 1];
				}
				else {
					board[move.GetToRow(), move.GetToCol()] = piece;
					currentHash = currentHash ^ zBoard[(5 * move.GetToRow()) + move.GetToCol(), (int)piece - 1];
				}
				board[move.GetFromRow(), move.GetFromCol()] = PIECES.NONE;
			}
		}//end of TryMove method

		public void UndoMove(Move move) {
			Removal removal = removals.Peek();
			if(move.GetToRow() == removal.GetRow() && move.GetToCol() == removal.GetCol()) {
				removal = removals.Pop();
				currentHash = currentHash ^ zBoard[(5 * move.GetToRow()) + move.GetToCol(), (int)board[move.GetToRow(), move.GetToCol()] - 1];
				if(board[move.GetToRow(), move.GetToCol()] == PIECES.P1_BISHOP) {
					board[move.GetFromRow(), move.GetFromCol()] = PIECES.P1_ROOK;
					currentHash = currentHash ^ zBoard[(5 * move.GetFromRow()) + move.GetFromCol(), (int)PIECES.P1_ROOK - 1];
				}
				else if(board[move.GetToRow(), move.GetToCol()] == PIECES.P2_BISHOP) {
					board[move.GetFromRow(), move.GetFromCol()] = PIECES.P2_ROOK;
					currentHash = currentHash ^ zBoard[(5 * move.GetFromRow()) + move.GetFromCol(), (int)PIECES.P2_ROOK - 1];
				}
				else if(board[move.GetToRow(), move.GetToCol()] == PIECES.P1_ROOK) {
					board[move.GetFromRow(), move.GetFromCol()] = PIECES.P1_KNIGHT;
					currentHash = currentHash ^ zBoard[(5 * move.GetFromRow()) + move.GetFromCol(), (int)PIECES.P1_KNIGHT - 1];
				}
				else if(board[move.GetToRow(), move.GetToCol()] == PIECES.P2_ROOK) {
					board[move.GetFromRow(), move.GetFromCol()] = PIECES.P2_KNIGHT;
					currentHash = currentHash ^ zBoard[(5 * move.GetFromRow()) + move.GetFromCol(), (int)PIECES.P2_KNIGHT - 1];
				}
				else if(board[move.GetToRow(), move.GetToCol()] == PIECES.P1_KNIGHT) {
					board[move.GetFromRow(), move.GetFromCol()] = PIECES.P1_BISHOP;
					currentHash = currentHash ^ zBoard[(5 * move.GetFromRow()) + move.GetFromCol(), (int)PIECES.P1_BISHOP - 1];
				}
				else if(board[move.GetToRow(), move.GetToCol()] == PIECES.P2_KNIGHT) {
					board[move.GetFromRow(), move.GetFromCol()] = PIECES.P2_BISHOP;
					currentHash = currentHash ^ zBoard[(5 * move.GetFromRow()) + move.GetFromCol(), (int)PIECES.P2_BISHOP - 1];
				}
				else {
					board[move.GetFromRow(), move.GetFromCol()] = board[move.GetToRow(), move.GetToCol()];
					currentHash = currentHash ^ zBoard[(5 * move.GetFromRow()) + move.GetFromCol(), (int)board[move.GetToRow(), move.GetToCol()] - 1];
				}
				board[removal.GetRow(), removal.GetCol()] = removal.GetPiece();
				if(removal.GetPiece() != PIECES.NONE) {
					currentHash = currentHash ^ zBoard[(5 * removal.GetRow()) + removal.GetCol(), (int)removal.GetPiece() - 1];
				}
			}
		}//end of UndoMove method

		public bool IsGameOver() {
			bool returnValue = false;
			bool p1KingFound = false;
			for(int i = 0; i < 8; i++) {
				for(int j = 0; j < 6; j++) {
					if(board[i, j] == PIECES.P1_KING) {
						p1KingFound = true;
						break;
					}
				}
				if(p1KingFound) {
					break;
				}
			}
			if(!p1KingFound || MoveGenerator(PLAYERS.P1).Count == 0) {
				winner = PLAYERS.P2;
				returnValue = true;
			}
			bool p2KingFound = false;
			for(int i = 0; i < 8; i++) {
				for(int j = 0; j < 6; j++) {
					if(board[i, j] == PIECES.P2_KING) {
						p2KingFound = true;
						break;
					}
				}
				if(p2KingFound) {
					break;
				}
			}
			if(!p1KingFound || MoveGenerator(PLAYERS.P1).Count == 0) {
				winner = PLAYERS.P2;
				returnValue = true;
			}
			if(!p2KingFound || MoveGenerator(PLAYERS.P2).Count == 0) {
				winner = PLAYERS.P1;
				returnValue = true;
			}
			return returnValue;
		}//end of IsGameOver method

		public List<Move> MoveGenerator(PLAYERS player) {
			List<Move> returnValue;
			if(player == PLAYERS.P1) {
				returnValue = MoveGenP1();
			}
			else if(player == PLAYERS.P2) {
				returnValue = MoveGenP2();
			}
			else {
				returnValue = new List<Move>();
			}
			return returnValue;
		}//end of MoveGenerator method

		private List<Move> MoveGenP1() {
			List<Move> returnValue = new List<Move>();
			for(int i = 0; i < 8; i++) {
				for(int j = 0; j < 6; j++) {
//					Console.WriteLine($"board[{i},{j}]={board[i,j]}");
					if(board[i, j] != PIECES.NONE) {
						if(board[i, j] == PIECES.P1_KING) {
							if(OnBoard(i, j + 1)) {
								if(Empty(i, j + 1) || !OwnPieceP1(i, j + 1)) {
									returnValue.Add(new Move(i, j, i, j + 1));
								}
							}
							if(OnBoard(i, j - 1)) {
								if(!Empty(i, j - 1) && !OwnPieceP1(i, j - 1)) {
									returnValue.Add(new Move(i, j, i, j - 1));
								}
							}
						}
						else if(board[i, j] == PIECES.P1_PAWN) {
							if(OnBoard(i + 1, j)) {
								if(Empty(i + 1, j)) {
									returnValue.Add(new Move(i, j, i + 1, j));
								}
							}
							if(OnBoard(i + 1, j - 1)) {
								if(!Empty(i + 1, j - 1) && !OwnPieceP1(i + 1, j - 1)) {
									returnValue.Add(new Move(i, j, i + 1, j - 1));
								}
							}
							if(OnBoard(i + 1, j + 1)) {
								if(!Empty(i + 1, j + 1) && !OwnPieceP1(i + 1, j + 1)) {
									returnValue.Add(new Move(i, j, i + 1, j + 1));
								}
							}
						}
						else if(board[i, j] == PIECES.P1_KNIGHT) {
							if(OnBoard(i + 1, j - 2)) {
								if(Empty(i + 1, j - 2) || !OwnPieceP1(i + 1, j - 2)) {
									returnValue.Add(new Move(i, j, i + 1, j - 2));
								}
							}
							if(OnBoard(i + 2, j - 1)) {
								if(Empty(i + 2, j - 1) || !OwnPieceP1(i + 2, j - 1)) {
									returnValue.Add(new Move(i, j, i + 2, j - 1));
								}
							}
							if(OnBoard(i + 1, j + 2)) {
								if(Empty(i + 1, j + 2) || !OwnPieceP1(i + 1, j + 2)) {
									returnValue.Add(new Move(i, j, i + 1, j + 2));
								}
							}
							if(OnBoard(i + 2, j + 1)) {
								if(Empty(i + 2, j + 1) || !OwnPieceP1(i + 2, j + 1)) {
									returnValue.Add(new Move(i, j, i + 2, j + 1));
								}
							}
							if(OnBoard(i - 1, j - 2)) {
								if(!Empty(i - 1, j - 2) && !OwnPieceP1(i - 1, j - 2)) {
									returnValue.Add(new Move(i, j, i - 1, j - 2));
								}
							}
							if(OnBoard(i - 2, j - 1)) {
								if(!Empty(i - 2, j - 1) && !OwnPieceP1(i - 2, j - 1)) {
									returnValue.Add(new Move(i, j, i - 2, j - 1));
								}
							}
							if(OnBoard(i - 1, j + 2)) {
								if(!Empty(i - 1, j + 2) && !OwnPieceP1(i - 1, j + 2)) {
									returnValue.Add(new Move(i, j, i - 1, j + 2));
								}
							}
							if(OnBoard(i - 2, j + 1)) {
								if(!Empty(i - 2, j + 1) && !OwnPieceP1(i - 2, j + 1)) {
									returnValue.Add(new Move(i, j, i - 2, j + 1));
								}
							}
						}
						else if(board[i, j] == PIECES.P1_BISHOP) {
							int downRightI = i + 1;
							int downRightJ = j + 1;
							while(OnBoard(downRightI, downRightJ) && !OwnPieceP1(downRightI, downRightJ)) {
								returnValue.Add(new Move(i, j, downRightI, downRightJ));
								if(Empty(downRightI, downRightJ)) {
									downRightI++;
									downRightJ++;
								}
								else {
									break;
								}
							}
							int downLeftI = i + 1;
							int downLeftJ = j - 1;
							while(OnBoard(downLeftI, downLeftJ) && !OwnPieceP1(downLeftI, downLeftJ)) {
								returnValue.Add(new Move(i, j, downLeftI, downLeftJ));
								if(Empty(downLeftI, downLeftJ)) {
									downLeftI++;
									downLeftJ--;
								}
								else {
									break;
								}
							}
							int upLeftI = i - 1;
							int upLeftJ = j - 1;
							while(OnBoard(upLeftI, upLeftJ) && !OwnPieceP1(upLeftI, upLeftJ)) {
								if(Empty(upLeftI, upLeftJ)) {
									upLeftI--;
									upLeftJ--;
								}
								else {
									returnValue.Add(new Move(i, j, upLeftI, upLeftJ));
									break;
								}
							}
							int upRightI = i - 1;
							int upRightJ = j + 1;
							while(OnBoard(upRightI, upRightJ) && !OwnPieceP1(upRightI, upRightJ)) {
								if(Empty(upRightI, upRightJ)) {
									upRightI--;
									upRightJ++;
								}
								else {
									returnValue.Add(new Move(i, j, upRightI, upRightJ));
									break;
								}
							}
						}
						else if(board[i, j] == PIECES.P1_ROOK) {
							int downI = i + 1;
							while(OnBoard(downI, j) && !OwnPieceP1(downI, j)) {
								returnValue.Add(new Move(i, j, downI, j));
								if(Empty(downI, j)) {
									downI++;
								}
								else {
									break;
								}
							}
							int upI = i - 1;
							while(OnBoard(upI, j) && !OwnPieceP1(upI, j)) {
								if(Empty(upI, j)) {
									upI--;
								}
								else {
									returnValue.Add(new Move(i, j, upI, j));
									break;
								}
							}
							int rightJ = j + 1;
							while(OnBoard(i, rightJ) && !OwnPieceP1(i, rightJ)) {
								returnValue.Add(new Move(i, j, i, rightJ));
								if(Empty(i, rightJ)) {
									rightJ++;
								}
								else {
									break;
								}
							}
							int leftJ = j - 1;
							while(OnBoard(i, leftJ) && !OwnPieceP1(i, leftJ)) {
								returnValue.Add(new Move(i, j, i, leftJ));
								if(Empty(i, leftJ)) {
									leftJ--;
								}
								else {
									break;
								}
							}
						}
					}
				}
			}
			return returnValue;
		}//end of MoveGenP1 method

		private List<Move> MoveGenP2() {
			List<Move> returnValue = new List<Move>();
			for(int i = 0; i < 8; i++) {
				for(int j = 0; j < 6; j++) {
//					Console.WriteLine($"board[{i},{j}]={board[i, j]}");
					if(board[i, j] != PIECES.NONE) {
						if(board[i, j] == PIECES.P2_KING) {
							if(OnBoard(i, j - 1)) {
								if(Empty(i, j - 1) || !OwnPieceP2(i, j - 1)) {
									returnValue.Add(new Move(i, j, i, j - 1));
								}
							}
							if(OnBoard(i, j + 1)) {
								if(!Empty(i, j + 1) && !OwnPieceP2(i, j + 1)) {
									returnValue.Add(new Move(i, j, i, j + 1));
								}
							}
						}
						else if(board[i, j] == PIECES.P2_PAWN) {
							if(OnBoard(i - 1, j)) {
								if(Empty(i - 1, j)) {
									returnValue.Add(new Move(i, j, i - 1, j));
								}
							}
							if(OnBoard(i - 1, j - 1)) {
								if(!Empty(i - 1, j - 1) && !OwnPieceP2(i - 1, j - 1)) {
									returnValue.Add(new Move(i, j, i - 1, j - 1));
								}
							}
							if(OnBoard(i - 1, j + 1)) {
								if(!Empty(i - 1, j + 1) && !OwnPieceP2(i - 1, j + 1)) {
									returnValue.Add(new Move(i, j, i - 1, j + 1));
								}
							}
						}
						else if(board[i, j] == PIECES.P2_KNIGHT) {
							if(OnBoard(i - 1, j - 2)) {
								if(Empty(i - 1, j - 2) || !OwnPieceP2(i - 1, j - 2)) {
									returnValue.Add(new Move(i, j, i - 1, j - 2));
								}
							}
							if(OnBoard(i - 2, j - 1)) {
								if(Empty(i - 2, j - 1) || !OwnPieceP2(i - 2, j - 1)) {
									returnValue.Add(new Move(i, j, i - 2, j - 1));
								}
							}
							if(OnBoard(i - 1, j + 2)) {
								if(Empty(i - 1, j + 2) || !OwnPieceP2(i - 1, j + 2)) {
									returnValue.Add(new Move(i, j, i - 1, j + 2));
								}
							}
							if(OnBoard(i - 2, j + 1)) {
								if(Empty(i - 2, j + 1) || !OwnPieceP2(i - 2, j + 1)) {
									returnValue.Add(new Move(i, j, i - 2, j + 1));
								}
							}
							if(OnBoard(i + 1, j - 2)) {
								if(!Empty(i + 1, j - 2) && !OwnPieceP2(i + 1, j - 2)) {
									returnValue.Add(new Move(i, j, i + 1, j - 2));
								}
							}
							if(OnBoard(i + 2, j - 1)) {
								if(!Empty(i + 2, j - 1) && !OwnPieceP2(i + 2, j - 1)) {
									returnValue.Add(new Move(i, j, i + 2, j - 1));
								}
							}
							if(OnBoard(i + 1, j + 2)) {
								if(!Empty(i + 1, j + 2) && !OwnPieceP2(i + 1, j + 2)) {
									returnValue.Add(new Move(i, j, i + 1, j + 2));
								}
							}
							if(OnBoard(i + 2, j + 1)) {
								if(!Empty(i + 2, j + 1) && !OwnPieceP2(i + 2, j + 1)) {
									returnValue.Add(new Move(i, j, i + 2, j + 1));
								}
							}
						}
						else if(board[i, j] == PIECES.P2_BISHOP) {
							int upLeftI = i - 1;
							int upLeftJ = j - 1;
							while(OnBoard(upLeftI, upLeftJ) && !OwnPieceP2(upLeftI, upLeftJ)) {
								returnValue.Add(new Move(i, j, upLeftI, upLeftJ));
								if(Empty(upLeftI, upLeftJ)) {
									upLeftI--;
									upLeftJ--;
								}
								else {
									break;
								}
							}
							int upRightI = i - 1;
							int upRightJ = j + 1;
							while(OnBoard(upRightI, upRightJ) && !OwnPieceP2(upRightI, upRightJ)) {
								returnValue.Add(new Move(i, j, upRightI, upRightJ));
								if(Empty(upRightI, upRightJ)) {
									upRightI--;
									upRightJ++;
								}
								else {
									break;
								}
							}
							int downRightI = i + 1;
							int downRightJ = j + 1;
							while(OnBoard(downRightI, downRightJ) && !OwnPieceP2(downRightI, downRightJ)) {
								if(Empty(downRightI, downRightJ)) {
									downRightI++;
									downRightJ++;
								}
								else {
									returnValue.Add(new Move(i, j, downRightI, downRightJ));
									break;
								}
							}
							int downLeftI = i + 1;
							int downLeftJ = j - 1;
							while(OnBoard(downLeftI, downLeftJ) && !OwnPieceP2(downLeftI, downLeftJ)) {
								if(Empty(downLeftI, downLeftJ)) {
									downLeftI++;
									downLeftJ--;
								}
								else {
									returnValue.Add(new Move(i, j, downLeftI, downLeftJ));
									break;
								}
							}
						}
						else if(board[i, j] == PIECES.P2_ROOK) {
							int upI = i - 1;
							while(OnBoard(upI, j) && !OwnPieceP2(upI, j)) {
								returnValue.Add(new Move(i, j, upI, j));
								if(Empty(upI, j)) {
									upI--;
								}
								else {
									break;
								}
							}
							int downI = i + 1;
							while(OnBoard(downI, j) && !OwnPieceP2(downI, j)) {
								if(Empty(downI, j)) {
									downI++;
								}
								else {
									returnValue.Add(new Move(i, j, downI, j));
									break;
								}
							}
							int rightJ = j + 1;
							while(OnBoard(i, rightJ) && !OwnPieceP2(i, rightJ)) {
								returnValue.Add(new Move(i, j, i, rightJ));
								if(Empty(i, rightJ)) {
									rightJ++;
								}
								else {
									break;
								}
							}
							int leftJ = j - 1;
							while(OnBoard(i, leftJ) && !OwnPieceP2(i, leftJ)) {
								returnValue.Add(new Move(i, j, i, leftJ));
								if(Empty(i, leftJ)) {
									leftJ--;
								}
								else {
									break;
								}
							}
						}
					}
				}
			}
			return returnValue;
		}//end of MoveGenP2 method

		private bool OnBoard(int row, int col) {
			bool returnValue = true;
			if(row < 0 || row > 7 || col < 0 || col > 5) {
				returnValue = false;
			}
			return returnValue;
		}//end of OnBoard method

		private bool Empty(int row, int col) {
			bool returnValue = false;
			if(board[row, col] == PIECES.NONE) {
				returnValue = true;
			}
			return returnValue;
		}//end of Empty method

		private bool OwnPieceP1(int row, int col) {
			bool returnValue = false;
			if(board[row, col] == PIECES.P1_BISHOP ||
				board[row, col] == PIECES.P1_KING ||
				board[row, col] == PIECES.P1_KNIGHT ||
				board[row, col] == PIECES.P1_ROOK ||
				board[row, col] == PIECES.P1_PAWN) {
				returnValue = true;
			}
//			Console.WriteLine($"OwnPiece={returnValue} for [{row},{col}] -> {board[row,col]}");
			return returnValue;
		}//end of OwnPieceP1 method

		private bool OwnPieceP2(int row, int col) {
			bool returnValue = false;
			if(board[row, col] == PIECES.P2_BISHOP ||
				board[row, col] == PIECES.P2_KING ||
				board[row, col] == PIECES.P2_KNIGHT ||
				board[row, col] == PIECES.P2_ROOK ||
				board[row, col] == PIECES.P2_PAWN) {
				returnValue = true;
			}
			return returnValue;
		}//end of OwnPieceP2 method

		public PIECES PieceAt(int row, int col) {
			if(row >= 0 && row < 8 && col >= 0 && col < 6) {
				return board[row, col];
			}
			else {
				return PIECES.NONE;
			}
		}//end of PieceAt method

		public Removal PeekRemoval() {
			return removals.Peek();
		}//end of PeekRemoval method

		public long GetCurrentHash() {
			return currentHash;
		}//end of GetCurrentHash method

		public PIECES[] GetBoardRow(int row) {
			PIECES[] returnValue = new PIECES[6];
			if(row >= 0 && row <= 7) {
				returnValue[0] = board[row, 0];
				returnValue[1] = board[row, 1];
				returnValue[2] = board[row, 2];
				returnValue[3] = board[row, 3];
				returnValue[4] = board[row, 4];
				returnValue[5] = board[row, 5];
			}
			else {
				returnValue[0] = PIECES.NONE;
				returnValue[1] = PIECES.NONE;
				returnValue[2] = PIECES.NONE;
				returnValue[3] = PIECES.NONE;
				returnValue[4] = PIECES.NONE;
				returnValue[5] = PIECES.NONE;
			}
			return returnValue;
		}//end of GetBoardRow method

		public PIECES[,] GetBoard() {
			return board.Clone() as Game.PIECES[,];
		}//end of GetBoard method

		public PLAYERS GetComputerPlayer() {
			return computer;
		}//end of GetComputerPlayer method

		public PLAYERS GetHumanPlayer() {
			return human;
		}//end of GetComputerPlayer method

		public ConsoleColor GetComputerColor() {
			return computerColor;
		}//end of GetComputerColor method

		public ConsoleColor GetHumanColor() {
			return humanColor;
		}//end of GetHumanColor method

		public bool GameInProgress() {
			return gameInProgress;
		}//end of GameInProgress method

		public PLAYERS GetWinner() {
			return winner;
		}//end of GetWinner method

		public void SetComputerPlayer(PLAYERS player) {
			computer = player;
		}//end of SetComputerPlayer method

		public void SetHumanPlayer(PLAYERS player) {
			human = player;
		}//end of SetHumanPlayer method

		public void SetComputerColor(ConsoleColor color) {
			computerColor = color;
		}//end of SetComputerColor method

		public void SetHumanColor(ConsoleColor color) {
			humanColor = color;
		}//end of SetHumanColor method

	}//end of Game class
}//end of Thompsor namespace