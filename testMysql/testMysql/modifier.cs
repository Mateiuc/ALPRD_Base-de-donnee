using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// Programme de la fenêtre 'Modifier' permettant d'insérer les nouvelles infos du client sélectionné 
// dans listView1 de la fenêtre principale.

namespace testMysql
{
    public partial class modifier : Form
    {   // Création de 6 variables string utilisées dans la focntion:
        // (private void modifierToolStripMenuItem_Click(object sender, EventArgs e)).
        public string Id { set { textBox7.Text = value; } }
        public string Nom { get { return textBox8.Text; } set { textBox8.Text = value; } }
        public string Prenom { get { return textBox9.Text; } set { textBox9.Text = value; } }
        public string Statut { get { return textBox10.Text; } set { textBox10.Text = value; } }
        public string Nfc { get { return textBox11.Text; } set { textBox11.Text = value; } }
        public string Vehicule { get { return textBox12.Text; } set { textBox12.Text = value; } }

        public modifier()
        {
            InitializeComponent();
        }

        // Pour lancer la modification dans la base de donnée:
        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
        }
    }
}
