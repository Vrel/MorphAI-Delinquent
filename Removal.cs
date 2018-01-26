using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MorphAI_Delinquent {

	/// <summary>
	/// Stores the details of a game piece that has been removed,  
	/// allowing for it to be reviewed or replaced later.
	/// </summary> 
	/// <para>
	/// Author: Robert Thompson
	/// Date: March 14, 2017
	/// </para>
	class Removal {

		private int row;
		private int col;
		private Game.PIECES piece;

		public Removal(int row, int col, Game.PIECES piece) {
			this.row = row;
			this.col = col;
			this.piece = piece;
		}//end of constructor

		public int GetRow() {
			return row;
		}//end of GetRow method

		public int GetCol() {
			return col;
		}//end of GetCol method

		public Game.PIECES GetPiece() {
			return piece;
		}//end of GetPiece method

	}//end of Removal class
	
}//end of Thompsor namespace