using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using MySql.Data.MySqlClient;

/* Remarques:
 * Dans les propriétés des listView, modifier 'FullRowSlect' à 'True' et 'Multiselect' à 'False'.
 * Dans les propriétés de F_accueil, 'FormBorderStyle' à 'FixedSingle'.
*/
namespace testMysql
{   
    public partial class F_accueil : Form
    { 
         //Variable servant à créer la commande de connection au serveur:
        string connectionString = "Server=192.168.0.101;Port=3306;Database=BD_parking;Uid=user;Pwd=pass;SslMode=none";
        bool connecte = false; // Variable qui informe si la coonection est active ou pas (True ou Flase).
        MySqlConnection connection;  // coonection = instance de la classe MySqlConnection.

        public F_accueil()
        {
            InitializeComponent();         
        }

        // ***** Fonctions Connection - déconnection : *****
        private void button1_Click(object sender, EventArgs e)
        {
            // Association de l'instance 'connection' à la vraible connectionString:
            connection = new MySqlConnection(connectionString);

            if (button1.Text == "Se connecter")
            {
                try // Traitement si exceptions
                {   //On vérifie si la connection est fermé:
                    if (connection.State == ConnectionState.Closed)
                    {  
                        connection.Open(); // On ouvre la connection.
                        connecte = true;   // On met à true car la connection est ouverte.
                        MessageBox.Show("Connecté !");
                        button1.Text = "Se deconnecter";
                    }
                }
                catch (MySqlException co) // Si erreur, affichage de l'erreur.
                {
                    MessageBox.Show(co.ToString());
                    MessageBox.Show("Non Connecté !");
                }
            }
            else
            {   // Pour fermer la connection au serveur:
                connection.Close();
                connecte = false; // On met à false car la connection est fermée.
               button1.Text = "Se connecter";
            }
        }

        //***** Fonction Ajouter : *****
        private void button2_Click(object sender, EventArgs e)
        {   // On vérifie que tout les textBox sont remplis (1 à 5):
            if (textBox1.Text == "") MessageBox.Show("Entrez un nom !");

            else if (textBox2.Text == "")  MessageBox.Show("Entrez un prénom !");

            else if (textBox3.Text == "") MessageBox.Show("Entrez le statut !");

            else if (textBox4.Text == "") MessageBox.Show("Entrez le code NFC !");

            else if (textBox5.Text == "")  MessageBox.Show("Entrez l'immatriculation !");

            else
            {
                if (connecte)  // Si on est connecté à la base de donnée.
                {
                    try // Gestion des exceptions.
                    {   // Commande SQL pour ajouter un client dans la base de donnée:
                        string query = "INSERT INTO TAB_clients(nomClients, prenomClients, statutClients, nfcClients, vehiculeClients) VALUES(@nom, @prenom, @statut, @nfc, @vehicule)";
                        MySqlCommand cmd = new MySqlCommand(query, connection);
                        cmd.Prepare();
                        //On récupére les infos des textBox et on les ajoutent à la commande SQL 'query':
                        cmd.Parameters.AddWithValue("@nom", textBox1.Text);
                        cmd.Parameters.AddWithValue("@prenom", textBox2.Text);
                        cmd.Parameters.AddWithValue("@statut", textBox3.Text);
                        cmd.Parameters.AddWithValue("@nfc", textBox4.Text);
                        cmd.Parameters.AddWithValue("@vehicule", textBox5.Text);
                        cmd.ExecuteNonQuery();  // On envoie la requête SQL au serveur.
                        cmd.Parameters.Clear();
                        MessageBox.Show("Ajouté !!!");
                    }
                    catch (MySql.Data.MySqlClient.MySqlException ex)
                    {
                        MessageBox.Show("Erreur ajouter" + ex.Message + ex.Number);
                    }
                }
                else MessageBox.Show ("Vous n'êtes pas connecté !");
            }
        }
        
        //***** Fonction affichage de la table TAB_clients : *****
        private void button3_Click(object sender, EventArgs e)
        {
            if (connecte)
            {
                listView1.Items.Clear(); // On efface les données affichées dans le contrôle listView1.
                                         // (listView1 permet d'afficher les différents clients sous
                                         // forme de tableau et colonnes.)  
                // Commande SQL pour afficher tous les clients de TAB_clients:
                string query = "SELECT * FROM TAB_clients";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                using (MySqlDataReader lecture = cmd.ExecuteReader())
                {
                    while (lecture.Read()) //Boucle qui permet d'afficher tout les clients.
                    {   // On récupere les infos envoyé par la base de donnée:
                        string ID = lecture["idClients"].ToString();
                        string Nom = lecture["nomClients"].ToString();
                        string Prenom = lecture["prenomClients"].ToString();
                        string Statut = lecture["statutClients"].ToString();
                        string Nfc = lecture["nfcClients"].ToString();
                        string Vehicule  = lecture["vehiculeClients"].ToString();
                        string Date = lecture["date_creationClients"].ToString();
                        //Affichage des clients dans la listView1: 
                        listView1.Items.Add(new ListViewItem(new[] { ID, Nom, Prenom, Statut, Nfc, Vehicule, Date }));
                    }
                }              
            }
            else
            {
                MessageBox.Show("Vous n'êtes pas connecté !");
            }
        }

        //***** Fonction supprimer une rangee de donnée de la table TAB_clients : *****
        // Ici on utilise le click droit de la souris et l'outil "ContextMenuStrip".
        private void supprimerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (connecte)
            {
                if (listView1.SelectedItems.Count > 0)  //count=13 si 13 lignes de donnée.
                {
                    ListViewItem element = listView1.SelectedItems[0]; // 0 ou erreur.
                    // recupère le contenu de la colonne 0 de la ligne élément ici l'idClients,
                    // exemple: si SubItems[2].text on récupérera le prénom du client :
                    string Id = element.SubItems[0].Text; 
                    // Requête SQL pour effacer un client dans TAB_clients en donnant son idClients:
                    string query = "DELETE FROM TAB_clients WHERE idClients=@id";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    // On additionne L'ID dans la requête:
                    cmd.Parameters.AddWithValue("@id", Id);
                    cmd.ExecuteNonQuery();
                    element.Remove();
                    MessageBox.Show("Element supprimé !");
                }
            }
        }

        //***** 2ème Fonction supprimer une rangee de donnée de la table TAB_clients : *****
        // Ici on utilise l'outil de base "button".
        private void button2_Click_1(object sender, EventArgs e)
        {
            if (connecte)
            {
                if (listView1.SelectedItems.Count > 0)  //count=13 si 13 lignes de donnée.
                {
                    ListViewItem element = listView1.SelectedItems[0]; // 0 ou erreur.
                    // recupère le contenu de la colonne 0 de la ligne element ici l'idClients.
                    string Id = element.SubItems[0].Text;
                    string query = "DELETE FROM TAB_clients WHERE idClients=@id";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@id", Id);
                    cmd.ExecuteNonQuery();
                    element.Remove();
                    MessageBox.Show("Element supprimé !");
                }
                else MessageBox.Show("Veuillez selectionner un clients !");
            }
        }

        /***** Fonction modifier clients dans la base de donnee : *****
         Utilisation du click droit souris puis click sur Modifier.
         Apparition d'une fenêtre 'Modifier' pour insérer les nouvelles données.
        */
        private void modifierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0) // Si on a une liste de client. 
            {   //  On récupére les données de chaque colonne du client selectionné :
                ListViewItem element = listView1.SelectedItems[0]; // 0 ou erreur.
                string ID = element.SubItems[0].Text;
                string nom = element.SubItems[1].Text;
                string prenom = element.SubItems[2].Text;
                string statut = element.SubItems[3].Text;
                string nfc = element.SubItems[4].Text;
                string vehicule = element.SubItems[5].Text;

                // Ci dessous on va modifer le Form nommé 'modifier' avec les données sélectionnées
                // dans 'listView' . 
                using (modifier m = new modifier() )
                {
                    m.Id = ID;   // 'm.Id' correspond à la variable crée dans 'modifer.cs'
                    m.Nom = nom; // Idem pour les données suivantes
                    m.Prenom = prenom;
                    m.Statut = statut;
                    m.Nfc = nfc;
                    m.Vehicule = vehicule;

                    //m.ShowDialog(); ligne qui ouvre le Form 'modifier'.

                    /* La condition suivante ouvre automatiquement la Fenêtre 'Modifier' même si la 
                       condition n'est pas remplie.
                       Si la condition est remplie, c'est à dire si l'on a clické sur le bouton 'Modifier'
                       de la Fenêtre 'Modifier' les valeurs du client id,nom etc... affichées dans les 'textBox'
                       seront envoyées dans la Base de donnée pour la modifier.
                    */
                    if (m.ShowDialog() == DialogResult.Yes)
                    {   // Préparation de la requête SQL et envoi à la table 'TAB_clients :
                        string query = "UPDATE TAB_clients SET  statutClients=@statut, nfcClients=@nfc, vehiculeClients=@vehicule WHERE idClients=@id ";
                        MySqlCommand cmd = new MySqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@id", ID);
                        cmd.Parameters.AddWithValue("@statut", m.Statut);
                        cmd.Parameters.AddWithValue("@nfc", m.Nfc);
                        cmd.Parameters.AddWithValue("@vehicule", m.Vehicule);
                        cmd.ExecuteNonQuery();
                        // mise à jour dela liste clients :
                        element.SubItems[3].Text = m.Statut;
                        element.SubItems[4].Text = m.Nfc;
                        element.SubItems[5].Text = m.Vehicule;
                        MessageBox.Show("Client modifié !");
                    }
                }
            }
        }

        //***** Fonction chercher un client par son nom : *****
        private void button4_Click(object sender, EventArgs e)
        {
            if (connecte)
            {
                listView1.Items.Clear();
                string query = "SELECT * FROM TAB_clients WHERE nomClients = @nom";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@nom", textBox6.Text);
                using (MySqlDataReader lecture = cmd.ExecuteReader())
                {
                    while (lecture.Read())
                    {
                        string ID = lecture["idClients"].ToString();
                        string Nom = lecture["nomClients"].ToString();
                        string Prenom = lecture["prenomClients"].ToString();
                        string Statut = lecture["statutClients"].ToString();
                        string Nfc = lecture["nfcClients"].ToString();
                        string Vehicule = lecture["vehiculeClients"].ToString();
                        string Date = lecture["date_creationClients"].ToString();

                        listView1.Items.Add(new ListViewItem(new[] { ID, Nom, Prenom, Statut, Nfc, Vehicule, Date }));
                    }
                }
            }
        }

        //***** Fonction chercher un client par son immatriculation : *****
        private void button5_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            string query = "SELECT * FROM TAB_clients WHERE vehiculeClients = @vehicule";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@vehicule", textBox6.Text);
            using (MySqlDataReader lecture = cmd.ExecuteReader())
            {
                while (lecture.Read())
                {
                    string ID = lecture["idClients"].ToString();
                    string Nom = lecture["nomClients"].ToString();
                    string Prenom = lecture["prenomClients"].ToString();
                    string Statut = lecture["statutClients"].ToString();
                    string Nfc = lecture["nfcClients"].ToString();
                    string Vehicule = lecture["vehiculeClients"].ToString();
                    string Date = lecture["date_creationClients"].ToString();

                    listView1.Items.Add(new ListViewItem(new[] { ID, Nom, Prenom, Statut, Nfc, Vehicule, Date }));
                }
            }
        }

        //***** Fonction affichage de la table TAB_camera : *****
        private void button6_Click(object sender, EventArgs e)
        {
            if (connecte)
            {
                listView2.Items.Clear();
                string query = "SELECT * FROM TAB_camera";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                using (MySqlDataReader lecture = cmd.ExecuteReader())
                {
                    while (lecture.Read())
                    {
                        string ID = lecture["idCamera"].ToString();
                        string plaque = lecture["plaqueCamera"].ToString();
                        string nom = lecture["nomCamera"].ToString();
                        string date = lecture["dateCamera"].ToString();
                        string heure = lecture["heureCamera"].ToString();
                        string autorisation = lecture["autorisationCamera"].ToString();

                        listView2.Items.Add(new ListViewItem(new[] { ID, plaque, nom, date, heure, autorisation }));
                    }
                }
            }
            else MessageBox.Show("Vous n'êtes pas connecté !");
        }

        //***** Fonction supprimer une rangee de donnée de la table TAB_camera : *****
        // Ici on utilise le click droit de la souris et l'outil "ContextMenuStrip (2)".
        private void supprimerToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (connecte)
            {
                if (listView2.SelectedItems.Count > 0)  //count=13 si 13 lignes de donnée.
                {
                    ListViewItem element = listView2.SelectedItems[0]; // 0 ou erreur.
                    // recupère le contenu de la colonne 0 de la ligne élément ici l'idClients.
                    string Id = element.SubItems[0].Text;
                    string query = "DELETE FROM TAB_camera WHERE idCamera=@id";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@id", Id);
                    cmd.ExecuteNonQuery();
                    element.Remove();
                    MessageBox.Show("Element supprimé !");
                }
            }
        }

        //***** 2ème Fonction supprimer une rangee de donnée de la table TAB_camera : *****
        // Ici on utilise l'outil de base "button".
        private void button7_Click(object sender, EventArgs e)
        {
            if (connecte)
            {
                if (listView2.SelectedItems.Count > 0)  //count=13 si 13 lignes de donnée.
                {
                    ListViewItem element = listView2.SelectedItems[0]; // 0 ou erreur.
                    // recupère le contenu de la colonne 0 de la ligne element ici l'idClients.
                    string Id = element.SubItems[0].Text;
                    string query = "DELETE FROM TAB_clients WHERE idClients=@id";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@id", Id);
                    cmd.ExecuteNonQuery();
                    element.Remove();
                    MessageBox.Show("Element supprimé !");
                }
                else MessageBox.Show("Veuillez selectionner un clients !");
            }
           else MessageBox.Show("Vous n'êtes pas connecté !");
        }

        //***** Fonction chercher un enregistrement par son nom dans TAB_camera: *****
        private void button8_Click(object sender, EventArgs e)
        {
            listView2.Items.Clear();
            string query = "SELECT * FROM TAB_camera WHERE nomCamera = @nom";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@nom", textBox8.Text);
            using (MySqlDataReader lecture = cmd.ExecuteReader())
            {
                while (lecture.Read())
                {
                    string ID = lecture["idCamera"].ToString();
                    string plaque = lecture["plaqueCamera"].ToString();
                    string Nom = lecture["nomCamera"].ToString();
                    string date = lecture["dateCamera"].ToString();
                    string heure = lecture["heureCamera"].ToString();
                    string autorisation = lecture["autorisationCamera"].ToString();

                    listView2.Items.Add(new ListViewItem(new[] { ID, plaque, Nom, date, heure, autorisation }));
                }
            }
        }

        //***** Fonction chercher un enregistrement par son immatriculation dans TAB_camera: *****
        private void button9_Click(object sender, EventArgs e)
        {
            if (connecte)
            {
                listView2.Items.Clear();
                string query = "SELECT * FROM TAB_camera WHERE plaqueCamera = @plaque";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@plaque", textBox9.Text);
                using (MySqlDataReader lecture = cmd.ExecuteReader())
                {
                    while (lecture.Read())
                    {
                        string ID = lecture["idCamera"].ToString();
                        string plaque = lecture["plaqueCamera"].ToString();
                        string Nom = lecture["nomCamera"].ToString();
                        string date = lecture["dateCamera"].ToString();
                        string heure = lecture["heureCamera"].ToString();
                        string autorisation = lecture["autorisationCamera"].ToString();

                        listView2.Items.Add(new ListViewItem(new[] { ID, plaque, Nom, date, heure, autorisation }));
                    }
                }
            }
            else MessageBox.Show("Vous n'êtes pas connecté !");
        }

        //***** Fonction chercher un enregistrement entre 2 dates dans TAB_camera: *****
        // Il faut ici modifier les prompriétés Format=Custom et CustomFormat=yyyy-MM-dd
        private void button10_Click(object sender, EventArgs e)
        {
            if (connecte)
                { 
                    listView2.Items.Clear();
                    string query = "SELECT * FROM TAB_camera WHERE dateCamera BETWEEN @date1 AND @date2";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@date1", dateTimePicker1.Text);
                    cmd.Parameters.AddWithValue("@date2", dateTimePicker2.Text);

                    using (MySqlDataReader lecture = cmd.ExecuteReader())
                        {
                            while (lecture.Read())
                                {
                                    string ID = lecture["idCamera"].ToString();
                                    string plaque = lecture["plaqueCamera"].ToString();
                                    string Nom = lecture["nomCamera"].ToString();
                                    string date = lecture["dateCamera"].ToString();
                                    string heure = lecture["heureCamera"].ToString();
                                    string autorisation = lecture["autorisationCamera"].ToString();

                                    listView2.Items.Add(new ListViewItem(new[] { ID, plaque, Nom, date, heure, autorisation }));
                                }
                         }
                }
            else MessageBox.Show("Vous n'êtes pas connecté !");
        }

        //***** Sauvegarder la base de donnée : *****
        private void button11_Click(object sender, EventArgs e)
        {
            string constring = "server=192.168.0.101;user=user;pwd=pass;database=BD_parking;";
            string file = "D:\\backup.sql";
            using (MySqlConnection conn = new MySqlConnection(constring))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    using (MySqlBackup mb = new MySqlBackup(cmd))
                    {
                        cmd.Connection = conn;
                        conn.Open();
                        mb.ExportToFile(file);
                        conn.Close();
                    }
                }
            }
        }

        //***** Restaurer la base de donnée : *****
        private void button12_Click(object sender, EventArgs e)
        {
            string constring = "server=192.168.0.101;user=user;pwd=pass;database=BD_parking;";
            string file = "D:\\backup.sql";
            using (MySqlConnection conn = new MySqlConnection(constring))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    using (MySqlBackup mb = new MySqlBackup(cmd))
                    {
                        cmd.Connection = conn;
                        conn.Open();
                        mb.ImportFromFile(file);
                        conn.Close();
                    }
                }
            }
        }
    }
}
