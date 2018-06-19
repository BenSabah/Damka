namespace GuiDamka
{
    using System;
    using System.Windows.Forms;

    public sealed partial class MainScreenForm : Form
    {
        int m_size = 8;
        public string m_player1 = "player1";
        string m_player2 = "player2";

        public MainScreenForm()
        {
            this.InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            DamkaBoardForm damkaGui = new DamkaBoardForm(this.m_size, this.m_player1, this.m_player2);
            damkaGui.Closed += (s, args) => this.Close();
            damkaGui.ShowDialog();
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.m_player1 = (sender as TextBox)?.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            this.m_player2 = (sender as TextBox)?.Text;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.textBox2.Enabled = (sender as CheckBox).Checked;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            this.m_size = 6;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            this.m_size = 8;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            this.m_size = 10;
        }
    }
}