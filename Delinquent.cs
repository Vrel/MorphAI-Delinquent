using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MorphAI_Delinquent {

	/// <summary>
	/// Core class of Delinquent, containing Main method and primary game objects.
	/// </summary> 
	/// <para>
	/// Author: Robert Thompson
	/// Date: March 14, 2017
	/// </para>
	class Delinquent {

		private static Game game = null;
		private static GameInterface game_interface = null;
		private static DelinquentBrain brain = null;
		public static bool human_going_first = false;

		static void Main(string[] args) {
			GetGame().InitBoard();
			GetInterface().DisplayBoard();
			GetGame().StartGame();
			if(!human_going_first) {
				//determine computer move
				int[] computerMove = GetBrain().GetComputerMove();
				//commit move
				Console.Write("Computer: ");
				GetGame().EnterMove(computerMove);
				//display board
				GetInterface().DisplayBoard();
			}
			while(GetGame().GameInProgress()) {
				//user move
				//check if move legal (done in GetUserMove)
				int[] userMove = GetInterface().GetUserMove();
				//commit move
				Console.Write("Human: ");
				GetGame().EnterMove(userMove);
				//display board
				GetInterface().DisplayBoard();
				//check if game over
				if(GetGame().IsGameOver()) {
					break;
				}
				//determine computer move
				int[] computerMove = GetBrain().GetComputerMove();
				//commit move
				Console.Write("Computer: ");
				GetGame().EnterMove(computerMove);
				//display board
				GetInterface().DisplayBoard();
				//check if game over
				if(GetGame().IsGameOver()) {
					break;
				}
			}
			GetInterface().DisplayResult();
			Console.ReadLine();
		}//end of Main method

		public static Game GetGame() {
			if(game == null) {
				game = new Game();
			}
			return game;
		}//end of GetGame method

		public static GameInterface GetInterface() {
			if(game_interface == null) {
				game_interface = new GameInterface();
			}
			return game_interface;
		}//end of GetInterface method

		public static DelinquentBrain GetBrain() {
			if(brain == null) {
				brain = new DelinquentBrain();
			}
			return brain;
		}//end of GetBrain method

	}//end of Delinquent class

}//end of thompsor namespace