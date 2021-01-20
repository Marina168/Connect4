using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IA_Project
{
    public partial class GameForm : Form
    {
        private Rectangle[] boardColumns;
        private int[,] Board;
        private int turn;
        const int NO_COLUMNS = 7;
        const int NO_ROWS = 6;
        const int HUMAN = 1;
        const int COMPUTER = 2;
        const int EMPTY = 0;
        const int HUMAN_PIECE = 1;
        const int COMPUTER_PIECE = 2;
        const int WINDOW_LENGTH = 4;
        const int NONE = -1;
        const int COMPUTER_VALUE= -2;
        const int HUMAN_VALUE = -3;
        private bool GAME_OVER = false;
        private int depth = 0;
        



        private int[] window;
        public GameForm()
        {
            InitializeComponent();

            this.boardColumns = new Rectangle[7];
            this.Board = new int[NO_ROWS, NO_COLUMNS];
            this.turn = HUMAN;
        }

        //desenam tabla
        private void GameForm_Paint(Object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.Blue, 24, 24, 340, 300);
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (i == 0)
                    {
                        this.boardColumns[j] = new Rectangle(32 + 48 * j, 24, 32, 300);
                    }
                    e.Graphics.FillEllipse(Brushes.White, 32 + 48 * j, 32 + 48 * i, 32, 32);
                }
            }
        }

        private void GameBoard_MouseClick(Object sender, MouseEventArgs e)
        {
            if (depth != 0)
            {
                int columnIndex = columnNumber(e.Location);
                Console.WriteLine("ColumnIndex from human" + columnIndex);
                if (this.turn == HUMAN && GAME_OVER == false)
                {
                    if (columnIndex >= 0)
                    {
                        int rowIndex = getNextFreeRow(this.Board, columnIndex);
                        Console.WriteLine("Human" + rowIndex);
                        if (rowIndex >= 0)
                        {

                            Console.WriteLine("From human :" + isValidLocation(this.Board, columnIndex));
                            if (isValidLocation(this.Board, columnIndex))
                            {

                                drop_piece(this.Board, rowIndex, columnIndex, HUMAN_PIECE);
                                Console.WriteLine("Human:" + rowIndex + "   " + columnIndex);
                                if (winning_move(this.Board, HUMAN_PIECE))
                                {
                                    draw_board(this.Board);
                                    MessageBox.Show("Human Wins");
                                    Thread.Sleep(1000);
                                    this.Close();
                                    GAME_OVER = true;

                                }
                                this.turn = COMPUTER;
                                draw_board(this.Board);
                                computer(this.Board);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Alege alt rand");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Alege alta coloana");
                    }
                }
            }
            else
            {
                MessageBox.Show("Insert depth and press Start button to start the game");
            }
            

        }


        private void computer(int [,] board)
        {


            if (this.turn == COMPUTER && GAME_OVER == false)
            {

                int columnIndex = minMax(board, this.depth, int.MinValue, int.MaxValue, true)[0];

                if (columnIndex >= 0)
                {
                    int rowIndex = getNextFreeRow(this.Board, columnIndex);
                    if (rowIndex >= 0)
                    {

                        Console.WriteLine("From computer :" + isValidLocation(this.Board, columnIndex));
                        if (isValidLocation(this.Board, columnIndex))
                        {

                            Console.WriteLine("Computer:" + rowIndex + "   " + columnIndex);
                            drop_piece(this.Board, rowIndex, columnIndex, COMPUTER_PIECE);
                            if (winning_move(this.Board, COMPUTER_PIECE))
                            {
                                draw_board(this.Board);
                                MessageBox.Show("Computer Wins");
                                Thread.Sleep(1000);
                                this.Close();
                                GAME_OVER = true;

                            }
                            this.turn = HUMAN;
                            draw_board(this.Board);
                        }

                    }
                    else
                    {
                        MessageBox.Show("Game is over. No more valid moves!");

                    }
                }

            }
                   
           
        }

        private int columnNumber(Point mouse)
        {
            for (int i = 0; i < this.boardColumns.Length; i++)
            {
                if ((mouse.X >= this.boardColumns[i].X) && (mouse.Y >= this.boardColumns[i].Y))
                {
                    if (mouse.X <= this.boardColumns[i].X + this.boardColumns[i].Width && mouse.Y <= this.boardColumns[i].Y + this.boardColumns[i].Height)
                    {
                        return i;
                    }
                }

            }

            return -1;

        }

        // drop piece
        private void drop_piece(int [,] board,int row, int column, int piece)
        {
         
            board[row, column] = piece;

           // Console.WriteLine("Board ");
            //printBoard(board);
        }
        private void draw_board(int [,] board)
        {
            for(int c=0;c<NO_COLUMNS;c++)
            {
                for (int r = 0; r < NO_ROWS;r++)
                {
                    if (board[r, c] == HUMAN_PIECE)
                    {
                        Graphics g = this.CreateGraphics();
                        g.FillEllipse(Brushes.Red, 32 + 48 * c, 32 + 48 * r, 32, 32);
                    }
                    else if (board[r, c] == COMPUTER_PIECE)
                    {
                        Graphics g = this.CreateGraphics();
                        g.FillEllipse(Brushes.Yellow, 32 + 48 * c, 32 + 48 * r, 32, 32);

                    }
                }
            }

        }
        private bool isValidLocation(int [,] board,int column)
        {
            return board[0, column] == 0;
        }

        private int getNextFreeRow(int [,] board,int column)
        {
            for (int r = NO_ROWS-1; r >=0; r--)
            {
                if (board[r, column] == 0)
                {
                    return r;
                }

            }
            return -1;
        }
        private void printBoard(int [,] board)
        {
            for (int i = 0; i < NO_ROWS; i++)
            {
                for (int j = 0; j < NO_COLUMNS; j++)
                {
                    Console.Write(board[i, j]);
                }
                Console.WriteLine(" ");
            }

        }

        private int[] getValidLocations(int [,] board)
        {
            List<int> validLocations = new List<int>();
            for (int i = 0; i < NO_COLUMNS; i++)
            {
                if (isValidLocation(board,i))
                {
                    validLocations.Add(i);
                }
            }
            return validLocations.ToArray();
        }

        private bool winning_move(int [,] board,int piece)
        {
            //horizontal
            for (int c = 0; c < NO_COLUMNS-3; c++)
            {
                for (int r = 0; r < NO_ROWS; r++)
                {
                    if (board[r, c] == piece && board[r, c + 1] == piece && board[r, c + 2] == piece && board[r, c + 3] == piece)
                        return true;
                }
            }

            //vertical
            for (int c = 0; c < NO_COLUMNS; c++)
            {
                for (int r = 0; r < NO_ROWS-3; r++)
                {
                    if (board[r, c] == piece && board[r + 1, c] == piece && board[r + 2, c] == piece && board[r + 3, c] == piece)
                        return true;
                }
            }


            //positively sloped diagonals
            for (int c = 0; c < NO_COLUMNS-3; c++)
            {
                for (int r = 0; r < NO_ROWS-3; r++)
                {
                    if (board[r, c] == piece && board[r + 1, c + 1] == piece && board[r + 2, c + 2] == piece && board[r + 3, c + 3] == piece)
                        return true;
                }
            }

            //negatively sloped diagonals
            for (int c = 0; c < NO_COLUMNS - 3; c++)
            {
                for (int r = 3; r < NO_ROWS; r++)
                {
                    if (board[r, c] == piece && board[r - 1, c + 1] == piece && board[r - 2, c + 2] == piece && board[r - 3, c + 3] == piece)
                        return true;
                }
            }
            return false;

        }

        private int count(int[] array, int p)
        {
            int cnt = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == p)
                {
                    cnt++;
                }
            }
            return cnt;
        }

        private int evaluate_board(int[] array, int piece)
        {
            int score = 0;
            int opp_piece = HUMAN_PIECE;
            if (piece == HUMAN_PIECE)
                opp_piece = COMPUTER_PIECE;

            if (count(array, piece) == 4)
            {
                score += 100;
            }
            else
                if (count(array, piece) == 3 && count(array, EMPTY) == 1)
            {
                score += 5;
            }
            else if (count(array, piece) == 2 && count(array, EMPTY) == 2)
            {
                score += 2;
            }

            if (count(array, opp_piece) == 3 && count(array, EMPTY) == 1)
            {
                score -= 4;
            }
            Console.WriteLine("Score" + score);
            return score;
        }

        public int[] subArray(int[] array, int start, int end)
        {
            int j = 0;
            int[] subArray = new int[10];
            for (int i = start; i < end; i++)
            {
                subArray[j] = array[i];
                j++;
            }
            return subArray;
        }
        private int score_position(int [,] board,int piece)
        {
            int score = 0;
            List<int> center_array = new List<int>();
            List<int> row_array = new List<int>();
            List<int> col_array = new List<int>();
            List<int> window = new List<int>();

            for (int i = 0; i < NO_ROWS; i++)
            {
                for (int j = 0; j <= 3; j++)
                {
                    center_array.Add(board[i, j]);
                }
            }

            score += count(center_array.ToArray(), piece) * 3;


            //score Horizontal 
            for(int r=0;r<NO_ROWS;r++)
            {

                for (int j = 0; j < NO_COLUMNS; j++)
                {

                    row_array.Add(board[r, j]);

                }

                for (int c = 0; c < NO_COLUMNS-3; c++)
                {


                    score += evaluate_board(subArray(row_array.ToArray(), c, c + 4), piece);

                }
            }

            //score vertical
            for (int c = 0; c < NO_COLUMNS; c++)
            {

                for (int j = 0; j < NO_ROWS; j++)
                {

                    col_array.Add(board[j, c]);

                }

                for (int r = 0; r < NO_ROWS-3; r++)
                {


                    score += evaluate_board(subArray(col_array.ToArray(), c, c + 4), piece);

                }
            }

            //score positive sloped diag
            for (int r = 0; r < NO_ROWS-3; r++)
            {

                for (int j = 0; j < NO_COLUMNS - 3; j++)
                {

                    for (int i = 0; i < WINDOW_LENGTH; i++)

                        window.Add(board[r + i, j + i]);
                    score += evaluate_board(window.ToArray(), piece);

                }
            }

            //score positive sloped diag
            for (int r = 0; r < NO_ROWS-3; r++)
            {

                for (int j = 0; j < NO_COLUMNS - 3; j++)
                {

                    for (int i = 0; i < WINDOW_LENGTH; i++)

                        window.Add(board[r + 3 - i, j + i]);
                    score += evaluate_board(window.ToArray(), piece);

                }
            }

            return score;
        }

        //
        private bool isTerminalNode(int [,] board)
        {
            return winning_move(board,HUMAN_PIECE) || winning_move(board,COMPUTER_PIECE) || getValidLocations(board).Length == 0;
        }
     
        
        private int [,] copyTo(int [,] board)
        {
            int[,] temp = new int[NO_ROWS, NO_COLUMNS];
            for(int i=0;i<NO_ROWS;i++)
            {
                for(int j=0;j<NO_COLUMNS;j++)
                {
                    temp[i, j] = board[i, j];
                }
            }
            return temp;
        }
        private  int [] minMax(int [,] board, int depth, int alpha, int beta, bool maximizingPlayer)
        {

            int[] valid_locations = getValidLocations(board);
            for(int i=0;i<getValidLocations(board).Length;i++)
            {
                Console.WriteLine(getValidLocations(board)[i]);
            }
            bool is_terminal = isTerminalNode(board);
            List<int> values = new List<int>();
            
            if(depth == 0 || is_terminal)
            {
                if (is_terminal)
                {
                    if (winning_move(board, COMPUTER_PIECE))
                    {
                        values.Add(NONE);
                        values.Add(COMPUTER_VALUE);
                        return values.ToArray();
                    }
                    else
                    {

                        if (winning_move(board, HUMAN_PIECE))
                        {
                            values.Add(NONE);
                            values.Add(HUMAN_VALUE);
                            return values.ToArray();
                        }
                        else
                        {
                            values.Add(NONE);
                            values.Add(0);
                            return values.ToArray();
                        }
                    }
                   
                }
                else
                {
                    values.Add(NONE);
                    values.Add(score_position(board, COMPUTER_PIECE));
                    return values.ToArray();
                }

            }

            if(maximizingPlayer)
            {
                int value = int.MinValue;
                Random rand = new Random();
                int index = rand.Next(valid_locations.Length);
                int column = valid_locations[index];
                int[,] temp = new int[NO_ROWS, NO_COLUMNS];
                for ( int c=0; c<valid_locations.Length;c++)
                {
                    int row = getNextFreeRow(board, valid_locations[c]);
                    Console.WriteLine("Minmax" + row);
                    temp = copyTo(board);
                    Console.WriteLine("Temp from min max");
                    printBoard(temp);
                    drop_piece(temp, row, valid_locations[c], COMPUTER_PIECE);
                    int new_score = minMax(temp, depth - 1, alpha, beta, false)[1];
                    if(new_score>value)
                    {
                        value = new_score;
                        column = valid_locations[c];
                    }
                    alpha = Math.Max(alpha, value);
                    if(alpha>=beta)
                    {
                        break;
                    }
                   
                }
                values.Add(column);
                values.Add(value);
                return values.ToArray();
            }
            else
            {
                int value = int.MaxValue;
                Random rand = new Random();
                int index = rand.Next(valid_locations.Length);
                int column = valid_locations[index];
                int[,] temp = new int[NO_ROWS, NO_COLUMNS];
                for (int c = 0; c < valid_locations.Length; c++)
                {
                    int row = getNextFreeRow(board, valid_locations[c]);
                   
                    temp =copyTo(board);
                    drop_piece(temp, row, valid_locations[c], HUMAN_PIECE);
                    int new_score = minMax(temp, depth - 1, alpha, beta, true)[1];
                    if (new_score < value)
                    {
                        value = new_score;
                        column = valid_locations[c];
                    }
                    beta = Math.Min(beta, value);
                    if (alpha >= beta)
                    {
                        break;
                    }

                }
                values.Add(column);
                values.Add(value);
                return values.ToArray();
            }

        }
         

        private void GameForm_Load(object sender, EventArgs e)
        {

        }

        private void GameForm_MouseUp(object sender, MouseEventArgs e)
        {
            //Console.WriteLine("nuuuuuuuuuuuu");
            //if(this.turn != HUMAN)
            //{

            //    computer(this.Board);
                
            //}
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("TextBox1" + textBox1.Text);
            String variable = textBox1.Text;
            try
            {
                this.depth = Int32.Parse(variable);
                if(this.depth<=0)
                {
                    MessageBox.Show("Depth should be positive number!");
                    textBox1.Clear();
                }
                else
                {
                    MessageBox.Show("You can start to play! Good luck!");
                }
            }
            catch
            {
                MessageBox.Show("Invalid depth format");
            }
            
        }
    }
}
