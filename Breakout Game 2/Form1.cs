using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Breakout_Game_2
{
    public partial class Form1 : Form
    {
        private Random rand = new Random();
        int ballSpeed = 3;
        int ballX = 1;
        int ballY = 1;
        int points = 0;
        int lives = 5;
        int bricksLeft = 36;
        int currentLevel = 1;
        int pointsNewLife = 0;
        bool paused = false;
        int count = 2;
        Control newControl;
        int newControlPosition = 0;
        SoundPlayer blockHit = new SoundPlayer(Breakout_Game_2.Properties.Resources.Hit1);
        SoundPlayer paddleHit = new SoundPlayer(Breakout_Game_2.Properties.Resources.PaddleHit);
        SoundPlayer gameOver = new SoundPlayer(Breakout_Game_2.Properties.Resources.quit);
        SoundPlayer speedIncereased = new SoundPlayer(Breakout_Game_2.Properties.Resources.speed);


        public Form1()
        {
            Thread t = new Thread(new ThreadStart(StartSplashScreen));
            t.Start();
            Thread.Sleep(4500);
            InitializeComponent();
            t.Abort();
        }
        public void StartSplashScreen()
        {
            Application.Run(new splashScreen());
        }

        private void startbutton_Click(object sender, EventArgs e)  // makes it so when you click the start button, it removes the button and starts the timer
        {
            setLives(2);
            showPoints();
            GameTimer.Enabled = true;
            startbutton.Visible = false;
            startPanel.Visible = false;
            panelEndGame.Visible = false;
            resetGame();
        }

        private void buttonRestart_Click(object sender, EventArgs e)
        {
            setLives(2);
            showPoints();
            GameTimer.Enabled = true;
            startbutton.Visible = false;
            startPanel.Visible = false;
            panelEndGame.Visible = false;
            resetGame();
            ballSpeed = 3;
            LocationReset();
        }

        private void resetGame()
        {

            //Color[] colors = { Color.Blue, Color.Red, Color.Purple, Color.Pink, Color.Orange, Color.Green };    // sets all the blocks to a random colour

            foreach (Control p in this.Controls)
            {
                if (p is PictureBox && (string)p.Tag == "Block")
                {
                    p.BackColor = Color.Purple;
                    p.Visible = true;

                }
                if (p is PictureBox && (string)p.Tag == "BlockH")
                {
                    p.BackColor = Color.Red;
                    p.Visible = true;

                }
            }
            if (lives <= 0)
            {
                panelEndGame.Visible = true;
                labelTotalPoints.Text = "Total Points: " + points;
                //startbutton.Visible = true;
                bricksLeft = 36;
                points = 0;
            }

            else

                if (bricksLeft == 0)
                {
                    bricksLeft = 36;
                    GameTimer.Enabled = false;
                    currentLevel++;
                    labelPointsNewLevel.Text = "Points: " + points;
                    labelCongratsNewLevel.Text = "Congratulations you \n have reached level " + currentLevel;
                    panelNewLevel.Visible = true;
                    buttonNextLevel.Visible = true;
                }
            resetBall();


        }

        private void setLives(int lives)
        {

            this.lives = lives; //this.lives refers to the global variable and lives is the local variable
            labelLives.Text = "Lives: " + lives;
        }

        private void showPoints()
        {
            labelPoints.Text = "Points: " + points;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            resetBall();
        }

        private void resetBall()       //centres ball
        {
            ballX = rand.Next(0, 2) * 2 - 1;
            ballY = rand.Next(0, 2) * 2 - 1;
            Ball.Left = (ClientRectangle.Width / 2) - (Ball.Width / 2);
            Ball.Top = (ClientRectangle.Height / 2) - (Ball.Height / 2);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            foreach (Control p in this.Controls)            //ball collision with block
            {
                if (p is PictureBox && (string)p.Tag == "Block")  // || (string)p.Tag == "Red"
                {
                    if (Ball.Bounds.IntersectsWith(p.Bounds) && p.Visible == true)
                    {
                        blockHit.Play();
                        if (p.Left > (Ball.Left + Ball.Width - ballSpeed) || (p.Left + p.Width - ballSpeed) < Ball.Left)
                        {
                            ballX = -ballX;
                        }
                        else
                        {
                            ballY = -ballY;
                        }
                        p.Visible = false;

                        addPoints(p.BackColor);
                        bricksLeft--;
                        break;
                    }
                }

                if (p is PictureBox && (string)p.Tag == "BlockH")
                {
                    if (Ball.Bounds.IntersectsWith(p.Bounds)) //  && p.Visible == true
                    {
                        speedIncereased.Play();
                        if (p.Left > (Ball.Left + Ball.Width - ballSpeed) || (p.Left + p.Width - ballSpeed) < Ball.Left)
                        {
                            ballX = -ballX;
                        }
                        else
                        {
                            ballY = -ballY;
                        }



                        newControl = p;
                        newControlPosition = p.Top;

                        if (count == 0)
                        {
                            timerH.Start();
                            timerH.Enabled = true;
                            //p.Visible = false;
                            addPoints(p.BackColor);
                            bricksLeft--;
                            count = 2;

                            break;
                        }
                        count--;
                    }
                }
            }

            Ball.Left = Ball.Left + ballSpeed * ballX;      //ball movement
            Ball.Top = Ball.Top + ballSpeed * ballY;


            if (Ball.Left < 0 || Ball.Left > ClientRectangle.Width - Ball.Width)        //ball collision with the edges
                ballX = -ballX;

            if (Ball.Top < 0 || Ball.Top > ClientRectangle.Height - Ball.Height)
                ballY = -ballY;

            if (Ball.Top + Ball.Height >= ClientRectangle.Height)
            {
                setLives(lives - 1);
                GameTimer.Enabled = false;



                if (lives == 0) // resets game after dying
                {
                    resetGame();
                }
                else
                {
                    buttonKeepGoing.Visible = true;
                    panelLost.Visible = true;
                    startbutton.Visible = false;
                    startPanel.Visible = false;
                    resetBall();
                }
            }

            if (Ball.Bounds.IntersectsWith(paddle.Bounds))
            {
                paddleHit.Play();
                ballY = -1;
            }

            if (bricksLeft == 0)    //resets game when a level is completed
            {
                resetGame();
            }
        }




        //-------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------
        //                                        Second Timer                                               ----
        //-------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------




        private void timerH_Tick(object sender, EventArgs e)
        {
            if (newControl.Name == "p1" || newControl.Name == "p2" || newControl.Name == "p3" || newControl.Name == "p4" || newControl.Name == "p5" || newControl.Name == "p6")
            {
                newControl.Width = 30;
                newControl.Height = 30;

                newControl.Visible = true;
                newControl.BackColor = Color.White;

                //for (int i = newControlPosition; newControl.Top + newControl.Height > ClientSize.Height; newControl.Top += 10)
                //{
                //    newControl.Top += 10;
                //}
                newControl.Top += 150;


                //if (newControl.Top + newControl.Height > ClientSize.Height)
                //{
                //    timerH.Stop();
                //    newControl.Visible = false;
                //}

                if (newControl.Bounds.IntersectsWith(paddle.Bounds))
                {
                    paddleHit.Play();
                    newControl.Visible = false;
                    paddle.Width = paddle.Width + 20;
                    timerH.Stop();
                }
                else
                {
                    newControl.Visible = false;
                    timerH.Stop();
                }
            }
        }

        private void addPoints(Color color) //adds points based on whether the ball hit a normal brick or a red brick
        {

            if (color.Equals(Color.Red)) // gives 20 points for red brick
            {
                points += 100;
                pointsNewLife += 2;
            }
            else
            {
                points += 10; // gives 10 points for any other color brick
                pointsNewLife += 10;
            }

            while (pointsNewLife >= 100) //gives a new life for every 100 points gained
            {
                setLives(lives + 1);
                pointsNewLife = 0;
            }
            showPoints();
        }

        private void PaddleMove(object sender, MouseEventArgs e) //moves paddle using the mouse
        {
            int newPos = e.X - paddle.Width / 2;

            if (paused == false)//pauses the paddle when "esc" is clicked
            {
                if (newPos < 0)
                {
                    newPos = 0;
                }

                if (newPos > ClientSize.Width - paddle.Width)
                {
                    newPos = ClientSize.Width - paddle.Width;
                }

                paddle.Left = newPos;
            }

        }

        private void buttonKeepGoing_Click(object sender, EventArgs e) //continues the game after dying
        {
            GameTimer.Enabled = true;
            buttonKeepGoing.Visible = false;
            panelLost.Visible = false;
            //ballSpeed = 3;
        }

        private void buttonNextLevel_Click(object sender, EventArgs e) //starts next level
        {
            GameTimer.Enabled = true;
            buttonNextLevel.Visible = false;
            panelNewLevel.Visible = false;
            resetGame();
            ballSpeed += ballSpeed;
            LocationReset();
        }
        private void LocationReset()
        {
            p1.Location = new Point(35, 223);
            p1.Size = new Size(84, 24);
            p2.Location = new Point(126, 193);
            p2.Size = new Size(84, 24);
            p3.Location = new Point(219, 165);
            p3.Size = new Size(84, 24);
            p4.Location = new Point(311, 135);
            p4.Size = new Size(84, 24);
            p5.Location = new Point(403, 106);
            p5.Size = new Size(84, 24);
            p6.Location = new Point(494, 77);
            p6.Size = new Size(84, 24);

        }


        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) //pauses the game when "esc" is clicked
        {
            if (keyData == Keys.Escape && GameTimer.Enabled == true) //ensures the game timer is enabled before you can pause
            {
                panelPaused.Visible = true;
                GameTimer.Enabled = false;
                paused = true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void buttonResume_Click(object sender, EventArgs e) //resumes the game
        {
            paused = false;
            GameTimer.Enabled = true;
            panelPaused.Visible = false;
        }

        private void btnPausedQuit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult d = MessageBox.Show("Are You Sure You Want to Exit", "Please Confirm!!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (d == DialogResult.OK)
            {
                Application.Exit();
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (MessageBox.Show("Are you sure you want to close!!!", "Form is going to close", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }

            }
            else
            {

            }
        }
    }
}
