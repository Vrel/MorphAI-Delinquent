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
	class GameInterface {

		private static readonly string[,] BOARD_MAP = {
			{ "A8", "B8", "C8", "D8", "E8", "F8" },
			{ "A7", "B7", "C7", "D7", "E7", "F7" },
			{ "A6", "B6", "C6", "D6", "E6", "F6" },
			{ "A5", "B5", "C5", "D5", "E5", "F5" },
			{ "A4", "B4", "C4", "D4", "E4", "F4" },
			{ "A3", "B3", "C3", "D3", "E3", "F3" },
			{ "A2", "B2", "C2", "D2", "E2", "F2" },
			{ "A1", "B1", "C1", "D1", "E1", "F1" }
		};

		private static readonly string[,] TRANSLATION_MAP = {
			{ "F1", "E1", "D1", "C1", "B1", "A1" },
			{ "F2", "E2", "D2", "C2", "B2", "A2" },
			{ "F3", "E3", "D3", "C3", "B3", "A3" },
			{ "F4", "E4", "D4", "C4", "B4", "A4" },
			{ "F5", "E5", "D5", "C5", "B5", "A5" },
			{ "F6", "E6", "D6", "C6", "B6", "A6" },
			{ "F7", "E7", "D7", "C7", "B7", "A7" },
			{ "F8", "E8", "D8", "C8", "B8", "A8" }
		};

		public GameInterface() {
			InitInterface();
			GetSetupInput();
		}//end of constructor

		private void GetSetupInput() {
			Console.WriteLine("Would you liked to go first? (yes/no)");
			bool selectionMade = false;
			do {
				String goFirstResponse = Console.ReadLine();
				if(input_affirmative(goFirstResponse)) {
					selectionMade = true;
					Delinquent.GetGame().SetComputerColor(ConsoleColor.DarkGreen);
					Delinquent.GetGame().SetHumanColor(ConsoleColor.White);
					Delinquent.human_going_first = true;
				}
				else if(input_negative(goFirstResponse)) {
					selectionMade = true;
					Delinquent.GetGame().SetComputerColor(ConsoleColor.White);
					Delinquent.GetGame().SetHumanColor(ConsoleColor.DarkGreen);
					Delinquent.human_going_first = false;
				}
				else {
					Console.WriteLine("Invalid selection. Please try again.");
				}
			}
			while(!selectionMade);
			Console.Clear();
//			Console.WriteLine($"Human=>{Delinquent.GetGame().GetHumanPlayer()}");
//			Console.WriteLine($"Computer=>{Delinquent.GetGame().GetComputerPlayer()}");
		}//end of GetSetupInput method

		public int[] GetUserMove() {
			int[] move;
			bool validMoveReceived = false;
			do {
				Console.ForegroundColor = ConsoleColor.Black;
				Console.WriteLine("Enter a move: ");
				string moveString = Console.ReadLine();
				move = map_move_string(moveString);
//				Console.WriteLine($"Move=[{move[0]}],[{move[1]}],[{move[2]}],[{move[3]}]");
				Game.PLAYERS human = Delinquent.GetGame().GetHumanPlayer();
//				Console.WriteLine($"Human={human}");
				validMoveReceived = Delinquent.GetGame().IsMoveLegal(move, human);
				if(!validMoveReceived) {
					Console.WriteLine("Entered move not legal. Please try again.");
				}
			}
			while(!validMoveReceived);
			return move;
		}//end of GetUserMove method

		private void InitInterface() {
			Console.Title = "Morph - Delinquent";
			Console.BackgroundColor = ConsoleColor.Gray;
			Console.ForegroundColor = ConsoleColor.Black;
			int windowWidth = 1;
			int windowHeight = 1;
			if(Console.LargestWindowWidth < 60) {
				windowWidth = Console.LargestWindowWidth;
			}
			else {
				windowWidth = 60;
			}
			if(Console.LargestWindowHeight < 32) {
				windowHeight = Console.LargestWindowHeight;
			}
			else {
				windowHeight = 32;
			}
			Console.SetWindowSize(windowWidth, windowHeight);
			Console.Clear();
		}//end of InitInterface method

		public void DisplayBoard() {
			Console.WriteLine();
			Console.WriteLine();
			DisplayRow1();
			DisplayRow2();
			DisplayRow3();
			DisplayRow4();
			DisplayRow5();
			DisplayRow6();
			DisplayRow7();
			DisplayRow8();
			DisplayColumLabels();
		}//end of DisplayBoard method

		private void DisplayRow8() {
			Console.ForegroundColor = ConsoleColor.Black;
			Console.Write("\t1 ");
			char[] row8 = get_row_chars(7);
			for(int i = 0; i < 6; i++) {
				if(row8[i] == get_char_for_piece(Game.PIECES.NONE)) {
					Console.ForegroundColor = ConsoleColor.Black;
				}
				else if(Char.IsUpper(row8[i])) {
					if(Delinquent.GetGame().GetComputerPlayer() == Game.PLAYERS.P1) {
						Console.ForegroundColor = Delinquent.GetGame().GetComputerColor();
					}
					else {
						Console.ForegroundColor = Delinquent.GetGame().GetHumanColor();
					}
				}
				else {
					if(Delinquent.GetGame().GetComputerPlayer() == Game.PLAYERS.P2) {
						Console.ForegroundColor = Delinquent.GetGame().GetComputerColor();
					}
					else {
						Console.ForegroundColor = Delinquent.GetGame().GetHumanColor();
					}
				}
				Console.Write(row8[i]);
				Console.Write(' ');
			}
			Console.WriteLine();
		}//end of DisplayRow8 method

		private void DisplayRow7() {
			Console.ForegroundColor = ConsoleColor.Black;
			Console.Write("\t2 ");
			char[] row7 = get_row_chars(6);
			for(int i = 0; i < 6; i++) {
				if(row7[i] == get_char_for_piece(Game.PIECES.NONE)) {
					Console.ForegroundColor = ConsoleColor.Black;
				}
				else if(Char.IsUpper(row7[i])) {
					if(Delinquent.GetGame().GetComputerPlayer() == Game.PLAYERS.P1) {
						Console.ForegroundColor = Delinquent.GetGame().GetComputerColor();
					}
					else {
						Console.ForegroundColor = Delinquent.GetGame().GetHumanColor();
					}
				}
				else {
					if(Delinquent.GetGame().GetComputerPlayer() == Game.PLAYERS.P2) {
						Console.ForegroundColor = Delinquent.GetGame().GetComputerColor();
					}
					else {
						Console.ForegroundColor = Delinquent.GetGame().GetHumanColor();
					}
				}
				Console.Write(row7[i]);
				Console.Write(' ');
			}
			Console.WriteLine();
		}//end of DisplayRow7 method

		private void DisplayRow6() {
			Console.ForegroundColor = ConsoleColor.Black;
			Console.Write("\t3 ");
			char[] row6 = get_row_chars(5);
			for(int i = 0; i < 6; i++) {
				if(row6[i] == get_char_for_piece(Game.PIECES.NONE)) {
					Console.ForegroundColor = ConsoleColor.Black;
				}
				else if(Char.IsUpper(row6[i])) {
					if(Delinquent.GetGame().GetComputerPlayer() == Game.PLAYERS.P1) {
						Console.ForegroundColor = Delinquent.GetGame().GetComputerColor();
					}
					else {
						Console.ForegroundColor = Delinquent.GetGame().GetHumanColor();
					}
				}
				else {
					if(Delinquent.GetGame().GetComputerPlayer() == Game.PLAYERS.P2) {
						Console.ForegroundColor = Delinquent.GetGame().GetComputerColor();
					}
					else {
						Console.ForegroundColor = Delinquent.GetGame().GetHumanColor();
					}
				}
				Console.Write(row6[i]);
				Console.Write(' ');
			}
			Console.WriteLine();
		}//end of DisplayRow6 method

		private void DisplayRow5() {
			Console.ForegroundColor = ConsoleColor.Black;
			Console.Write("\t4 ");
			char[] row5 = get_row_chars(4);
			for(int i = 0; i < 6; i++) {
				if(row5[i] == get_char_for_piece(Game.PIECES.NONE)) {
					Console.ForegroundColor = ConsoleColor.Black;
				}
				else if(Char.IsUpper(row5[i])) {
					if(Delinquent.GetGame().GetComputerPlayer() == Game.PLAYERS.P1) {
						Console.ForegroundColor = Delinquent.GetGame().GetComputerColor();
					}
					else {
						Console.ForegroundColor = Delinquent.GetGame().GetHumanColor();
					}
				}
				else {
					if(Delinquent.GetGame().GetComputerPlayer() == Game.PLAYERS.P2) {
						Console.ForegroundColor = Delinquent.GetGame().GetComputerColor();
					}
					else {
						Console.ForegroundColor = Delinquent.GetGame().GetHumanColor();
					}
				}
				Console.Write(row5[i]);
				Console.Write(' ');
			}
			Console.WriteLine();
		}//end of DisplayRow5 method

		private void DisplayRow4() {
			Console.ForegroundColor = ConsoleColor.Black;
			Console.Write("\t5 ");
			char[] row4 = get_row_chars(3);
			for(int i = 0; i < 6; i++) {
				if(row4[i] == get_char_for_piece(Game.PIECES.NONE)) {
					Console.ForegroundColor = ConsoleColor.Black;
				}
				else if(Char.IsUpper(row4[i])) {
					if(Delinquent.GetGame().GetComputerPlayer() == Game.PLAYERS.P1) {
						Console.ForegroundColor = Delinquent.GetGame().GetComputerColor();
					}
					else {
						Console.ForegroundColor = Delinquent.GetGame().GetHumanColor();
					}
				}
				else {
					if(Delinquent.GetGame().GetComputerPlayer() == Game.PLAYERS.P2) {
						Console.ForegroundColor = Delinquent.GetGame().GetComputerColor();
					}
					else {
						Console.ForegroundColor = Delinquent.GetGame().GetHumanColor();
					}
				}
				Console.Write(row4[i]);
				Console.Write(' ');
			}
			Console.WriteLine();
		}//end of DisplayRow4 method

		private void DisplayRow3() {
			Console.ForegroundColor = ConsoleColor.Black;
			Console.Write("\t6 ");
			char[] row3 = get_row_chars(2);
			for(int i = 0; i < 6; i++) {
				if(row3[i] == get_char_for_piece(Game.PIECES.NONE)) {
					Console.ForegroundColor = ConsoleColor.Black;
				}
				else if(Char.IsUpper(row3[i])) {
					if(Delinquent.GetGame().GetComputerPlayer() == Game.PLAYERS.P1) {
						Console.ForegroundColor = Delinquent.GetGame().GetComputerColor();
					}
					else {
						Console.ForegroundColor = Delinquent.GetGame().GetHumanColor();
					}
				}
				else {
					if(Delinquent.GetGame().GetComputerPlayer() == Game.PLAYERS.P2) {
						Console.ForegroundColor = Delinquent.GetGame().GetComputerColor();
					}
					else {
						Console.ForegroundColor = Delinquent.GetGame().GetHumanColor();
					}
				}
				Console.Write(row3[i]);
				Console.Write(' ');
			}
			Console.WriteLine();
		}//end of DisplayRow3 method

		private void DisplayRow2() {
			Console.ForegroundColor = ConsoleColor.Black;
			Console.Write("\t7 ");
			char[] row2 = get_row_chars(1);
			for(int i = 0; i < 6; i++) {
				if(row2[i] == get_char_for_piece(Game.PIECES.NONE)) {
					Console.ForegroundColor = ConsoleColor.Black;
				}
				else if(Char.IsUpper(row2[i])) {
					if(Delinquent.GetGame().GetComputerPlayer() == Game.PLAYERS.P1) {
						Console.ForegroundColor = Delinquent.GetGame().GetComputerColor();
					}
					else {
						Console.ForegroundColor = Delinquent.GetGame().GetHumanColor();
					}
				}
				else {
					if(Delinquent.GetGame().GetComputerPlayer() == Game.PLAYERS.P2) {
						Console.ForegroundColor = Delinquent.GetGame().GetComputerColor();
					}
					else {
						Console.ForegroundColor = Delinquent.GetGame().GetHumanColor();
					}
				}
				Console.Write(row2[i]);
				Console.Write(' ');
			}
			Console.WriteLine();
		}//end of DisplayRow2 method

		private void DisplayRow1() {
			Console.ForegroundColor = ConsoleColor.Black;
			Console.Write("\t8 ");
			char[] row1 = get_row_chars(0);
			for(int i = 0; i < 6; i++) {
				if(row1[i] == get_char_for_piece(Game.PIECES.NONE)) {
					Console.ForegroundColor = ConsoleColor.Black;
				}
				else if(Char.IsUpper(row1[i])) {
					if(Delinquent.GetGame().GetComputerPlayer() == Game.PLAYERS.P1) {
						Console.ForegroundColor = Delinquent.GetGame().GetComputerColor();
					}
					else {
						Console.ForegroundColor = Delinquent.GetGame().GetHumanColor();
					}
				}
				else {
					if(Delinquent.GetGame().GetComputerPlayer() == Game.PLAYERS.P2) {
						Console.ForegroundColor = Delinquent.GetGame().GetComputerColor();
					}
					else {
						Console.ForegroundColor = Delinquent.GetGame().GetHumanColor();
					}
				}
				Console.Write(row1[i]);
				Console.Write(' ');
			}
			Console.WriteLine();
		}//end of DisplayRow1 method

		private void DisplayColumLabels() {
			Console.WriteLine();
			Console.WriteLine("\t  A B C D E F");
			Console.WriteLine();
		}//end of DisplayColumLabels method

		public void DisplayResult() {
			string winner = "";
			Console.ForegroundColor = ConsoleColor.Red;
			if(Delinquent.GetGame().GetWinner() == Delinquent.GetGame().GetHumanPlayer()) {
				winner = "The Human";
			}
			else if(Delinquent.GetGame().GetWinner() == Delinquent.GetGame().GetComputerPlayer()) {
				winner = "Delinquent";
			}
			else {
				winner = "Nobody";
			}
			Console.WriteLine($"Game Over! {winner} has won!");
			Console.ForegroundColor = ConsoleColor.Black;
			Console.WriteLine();
			Console.WriteLine("Press [Enter] to quit.");
		}//end of DisplayResult method

		private static char[] get_row_chars(int row) {
			char[] returnValue = new char[6];
			Game.PIECES[] rowPieces = Delinquent.GetGame().GetBoardRow(row);
			for(int i=0; i < 6; i++) {
				returnValue[i] = get_char_for_piece(rowPieces[i]);
			}
			return returnValue;
		}//end of get_row_chars method

		public static char get_char_for_piece(Game.PIECES piece) {
			char returnValue = '0';
			switch(piece) {
			case Game.PIECES.NONE:
				returnValue = '-';
				break;
			case Game.PIECES.P1_KING:
				returnValue = 'K';
				break;
			case Game.PIECES.P1_KNIGHT:
				returnValue = 'N';
				break;
			case Game.PIECES.P1_BISHOP:
				returnValue = 'B';
				break;
			case Game.PIECES.P1_ROOK:
				returnValue = 'R';
				break;
			case Game.PIECES.P1_PAWN:
				returnValue = 'P';
				break;
			case Game.PIECES.P2_KING:
				returnValue = 'k';
				break;
			case Game.PIECES.P2_KNIGHT:
				returnValue = 'n';
				break;
			case Game.PIECES.P2_BISHOP:
				returnValue = 'b';
				break;
			case Game.PIECES.P2_ROOK:
				returnValue = 'r';
				break;
			case Game.PIECES.P2_PAWN:
				returnValue = 'p';
				break;
			}
			return returnValue;
		}//end of get_char_for_piece method

		public static int[] map_move_string(string move) {
			int[] returnValue = new int[4];
			if(move.Length == 4) {
				int[] fromLoc = map_board_location(move.Substring(0, 2));
				int[] toLoc = map_board_location(move.Substring(2, 2));
				returnValue[0] = fromLoc[0];
				returnValue[1] = fromLoc[1];
				returnValue[2] = toLoc[0];
				returnValue[3] = toLoc[1];
			}
			else {
				returnValue[0] = -1;
				returnValue[1] = -1;
				returnValue[2] = -1;
				returnValue[3] = -1;
			}
			return returnValue;
		}//end of map_move_string method

		public static int[] map_board_location(string loc) {
			int[] returnValue = new int[2];

			if(loc.Length == 2) {
				char[] chars = loc.ToUpper().ToCharArray();
				switch(chars[0]) {
				case 'A':
					returnValue[1] = 0;
					break;
				case 'B':
					returnValue[1] = 1;
					break;
				case 'C':
					returnValue[1] = 2;
					break;
				case 'D':
					returnValue[1] = 3;
					break;
				case 'E':
					returnValue[1] = 4;
					break;
				case 'F':
					returnValue[1] = 5;
					break;
				default:
					returnValue[1] = -1;
					break;
				}
				switch(chars[1]) {
				case '8':
					returnValue[0] = 0;
					break;
				case '7':
					returnValue[0] = 1;
					break;
				case '6':
					returnValue[0] = 2;
					break;
				case '5':
					returnValue[0] = 3;
					break;
				case '4':
					returnValue[0] = 4;
					break;
				case '3':
					returnValue[0] = 5;
					break;
				case '2':
					returnValue[0] = 6;
					break;
				case '1':
					returnValue[0] = 7;
					break;
				default:
					returnValue[1] = -1;
					break;
				}
			}
			else {
				returnValue[0] = -1;
				returnValue[1] = -1;
			}
			return returnValue;
		}//end of map_board_location(string) method

		public static string map_board_location(int row, int col) {
			string returnValue;
			if(row >= 0 && row <= 7 && col >= 0 && col <= 5) {
				returnValue = BOARD_MAP[row, col];
			}
			else {
				returnValue = "INVALID";
			}
			return returnValue;
		}//end of map_board_location(int,int) method

		public static string map_translate_location(int row, int col) {
			string returnValue;
			if(row >= 0 && row <= 7 && col >= 0 && col <= 5) {
				returnValue = TRANSLATION_MAP[row, col];
			}
			else {
				returnValue = "INVALID";
			}
			return returnValue;
		}//end of map_translate_location(int,int) method

		public static bool input_affirmative(String input) {
			bool returnValue = false;
			String inputCompare = input.ToLower();
			if(inputCompare == "yes" || inputCompare == "y") {
				returnValue = true;
			}
			return returnValue;
		}//end of input_affirmative method

		public static bool input_negative(String input) {
			bool returnValue = false;
			String inputCompare = input.ToLower();
			if(inputCompare == "no" || inputCompare == "n") {
				returnValue = true;
			}
			return returnValue;
		}//end of input_negative method

	}//end of GameInterface class

}//end of Thompsor namespace