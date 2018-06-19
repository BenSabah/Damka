namespace GuiDamka
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Windows.Forms;

    using DamkaGameEngine;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1308:VariableNamesMustNotBePrefixed", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1214:StaticReadonlyElementsMustAppearBeforeStaticNonReadonlyElements", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1311:StaticReadonlyFieldsMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]

    public sealed class DamkaBoardForm : Form
    {
        // Game-Engine related fields
        private static GameEngine s_gameEngine;
        private readonly int r_Size;

        // GUI related fields
        private readonly MyButton[,] r_Buttons;
        private MyButton m_selectedIndex;

        // Sizes for styling of the gui screen.
        private static readonly int sr_PaddingSize = 5;
        private static readonly Padding sr_ButtonPadding = new Padding(sr_PaddingSize);
        private static readonly int sr_ButtonDimension = 60;
        private static readonly Size sr_ButtonSize = new Size(sr_ButtonDimension, sr_ButtonDimension);

        public DamkaBoardForm(int i_Size, string i_Player1Name, string i_Player2Name)
        {
            s_gameEngine = new GameEngine(i_Size, i_Player1Name, i_Player2Name);

            this.r_Size = i_Size;
            this.r_Buttons = new MyButton[i_Size, i_Size];
            this.Text = string.Format("Damka {0} x {0}, {1} Vs. {2}", i_Size, i_Player1Name, i_Player2Name);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.AutoSize = true;
            this.KeyPreview = true;
            this.setupButtons();
        }

        protected override bool ProcessCmdKey(ref Message message, Keys keys)
        {
            if (keys == (Keys.Control | Keys.Q))
            {
                this.Close();
                return true;
            }

            return base.ProcessCmdKey(ref message, keys);
        }

        private void setupButtons()
        {
            int lineHeight = sr_PaddingSize;
            int lineWidth = sr_PaddingSize;
            GroupBox group = new GroupBox();
            this.Controls.Add(group);

            // Add NxN buttons to the form.
            for (int y = 0; y < this.r_Size; y++)
            {
                lineWidth = sr_PaddingSize;
                for (int x = 0; x < this.r_Size; x++)
                {
                    Point index = new Point(x, y);
                    MyButton b = new MyButton();
                    b.Location = new Point(lineWidth, lineHeight);
                    b.Index = new Point(x, y);
                    b.CheckedChanged += new EventHandler(this.OnClickingPlayingPiece);
                    b.CheckedChanged += new EventHandler(this.OnClickingEmptySpace);
                    b.CheckedChanged += new EventHandler(this.CheckIfFinished);
                    b.OriginalColor = ((x + y) % 2 == 0) ? Color.Gray : Color.Empty;

                    this.r_Buttons[x, y] = b;
                    group.Controls.Add(b);

                    // move the  to the next location
                    lineWidth += sr_ButtonSize.Width + sr_PaddingSize;
                }
                lineHeight += sr_ButtonDimension + sr_PaddingSize;
            }
            group.Size = new Size(lineWidth, lineHeight);
            this.refreshBoard();
        }

        private void CheckIfFinished(object sender, EventArgs e)
        {
            if (s_gameEngine.IsGameOver)
            {
                this.Hide();
                string goodbyeTitle = " Game Over !";
                string goodbyeMsg = $"{s_gameEngine.Winner.Name} WON!, press ok to close.";

                DialogResult res = MessageBox.Show(
                    goodbyeMsg,
                    goodbyeTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                this.Closed += (s, args) => this.Close();
            }
        }

        private void OnClickingPlayingPiece(object sender, EventArgs e)
        {
            // Check if button is an actual playing piece.
            MyButton selectedButton = sender as MyButton;
            if (selectedButton != null && selectedButton.Type != DamkaPieces.Type.None)
            {
                // mark current button to remember
                this.m_selectedIndex = selectedButton;

                // get the surrounding options and highlight them.
                List<Point> options = s_gameEngine.GetLandingOptions(selectedButton.Index);
                if (selectedButton.Checked)
                {
                    this.highlightLandingOptions(options);
                }
                else
                {
                    this.dimLandingOptions(options);
                }
                selectedButton.SelectButton();
            }
        }

        private void OnClickingEmptySpace(object sender, EventArgs e)
        {
            // Check if button is an empty space.
            this.m_selectedIndex.DimButton();
            this.m_selectedIndex.SelfReset();
            MyButton selectedButton = sender as MyButton;
            if (selectedButton.Type == DamkaPieces.Type.None)
            {
                s_gameEngine.MovePiece(this.m_selectedIndex.Index, selectedButton.Index);
                selectedButton.Checked = false;
                selectedButton.DimButton();
                this.refreshBoard();
            }
            selectedButton.SelectButton();
        }

        private void refreshBoard()
        {
            for (int y = 0; y < this.r_Size; y++)
            {
                for (int x = 0; x < this.r_Size; x++)
                {
                    this.r_Buttons[x, y].SelfReset();
                }
            }
        }

        private void flipValidButtons(bool i_activate)
        {
            for (int y = 0; y < this.r_Size; y++)
            {
                for (int x = 0; x < this.r_Size; x++)
                {
                    bool isEmptyButton = this.r_Buttons[x, y].Type == DamkaPieces.Type.None;
                    bool isActivePlayer = this.r_Buttons[x, y].Player == s_gameEngine.CurrentPlayer;
                    this.r_Buttons[x, y].Enabled = i_activate && !isEmptyButton && isActivePlayer;
                }
            }
        }

        private void highlightLandingOptions(List<Point> IO_options)
        {
            foreach (Point p in IO_options)
            {
                this.r_Buttons[p.X, p.Y].HighlightButton();
                this.r_Buttons[p.X, p.Y].Enabled = true;
            }
        }

        private void dimLandingOptions(List<Point> IO_options)
        {
            foreach (Point p in IO_options)
            {
                this.r_Buttons[p.X, p.Y].DimButton();
                this.r_Buttons[p.X, p.Y].Enabled = false;
            }
        }

        public sealed class MyButton : RadioButton
        {
            public Color OriginalColor { get; set; }

            public Color SelectedColor { get; set; }

            public Color HighlightedColor { get; set; }

            public Point Index { get; set; }

            public Player Player { get; set; }

            public DamkaPieces.Type Type { get; set; }

            public MyButton()
            {
                // general cosmetic settings.
                this.Margin = DamkaBoardForm.sr_ButtonPadding;
                this.Size = DamkaBoardForm.sr_ButtonSize;
                this.Font = new Font(Font.FontFamily, (int)(sr_ButtonDimension * 0.75));
                this.FlatStyle = FlatStyle.Flat;
                this.TextAlign = ContentAlignment.MiddleCenter;
                this.Appearance = Appearance.Button;
                this.FlatAppearance.BorderSize = 1;

                // Custom cosmetic settings.
                this.OriginalColor = Color.Empty;
                this.SelectedColor = Color.Blue;
                this.HighlightedColor = Color.LightGreen;
            }

            public void SelectButton()
            {
                this.BackColor = (this.Checked) ? this.SelectedColor : this.OriginalColor;
            }

            public void HighlightButton()
            {
                this.BackColor = this.HighlightedColor;
            }

            public void DimButton()
            {
                this.BackColor = this.OriginalColor;
            }

            public void SelfReset()
            {
                DamkaPieces.Type type = s_gameEngine.GetPieceByIndex(this.Index);
                this.Enabled = type != DamkaPieces.Type.None;
                this.Text = DamkaPieces.GetAdvancedType(type);
                this.Type = type;
                this.DimButton();

                // Enable only current player.
                bool isSameType = DamkaPieces.IsSameType(s_gameEngine.GetPieceByIndex(this.Index), s_gameEngine.CurrentPlayer.PlayerType);
                this.Enabled = isSameType;
            }
        }
    }
}
