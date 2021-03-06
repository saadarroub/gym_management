﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Media.Effects;
using System.Windows.Media.Animation;
using System.Text.RegularExpressions;

namespace GymWPF
{
    /// <summary>
    /// Logique d'interaction pour PayementsPage.xaml
    /// </summary>
    public partial class PayementsPage : Window
    {
        //declaration --------------------------------------
        SqlConnection cn = new SqlConnection("Data Source=.;Initial Catalog=NSS_Salle_Application;Integrated Security=True");
        SqlCommand cmd = new SqlCommand();
        SqlDataAdapter da = new SqlDataAdapter("", "Data Source=.;Initial Catalog=NSS_Salle_Application;Integrated Security=True");
        DataSet ds = new DataSet();
        SqlDataReader dr;
        //------------------------------------------------------


        string ConnectedSalle, ConnectedSport,id;
        DateTime last;
        MainApp dade;

        int count;
        public PayementsPage(MainApp d, string id, string ConnectedSalle, string ConnectedSport)
        {
            this.ConnectedSalle = ConnectedSalle;
            this.ConnectedSport = ConnectedSport;
            this.id = id;
            InitializeComponent();
            this.dade = d;
        }

        private void loaded()
        {
            NomTextBox.Language = System.Windows.Markup.XmlLanguage.GetLanguage("fr");

            cn.Open();
            cmd.Connection = cn;
            cmd.CommandText = "select p.IdPayment as ID,c.nom+' '+c.prenom as clinet,s.nom_Salle as salle,t.nom_Type as sport,convert(varchar, p.date_Payment, 103) as date,p.Prix as prix,c.IdClient,s.IdSalle,t.IdType from Payments p join Clients c on p.IdClient=c.IdClient join Salle s on p.IdSalle=s.IdSalle join Type_Sport t on p.IdType=t.IdType where c.IdClient='" + id+"' and s.IdSalle = '"+ConnectedSalle+"' and t.IdType = '"+ConnectedSport+ "' order by p.date_Payment DESC";
            dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            ListPayments.DataContext = dt;

           // count = dt.Rows.Count;          

            cn.Close();


        }
        public void coutItems()
        {
            count = ListPayments.Items.Count;

            cn.Open();
            cmd.Connection = cn;
            if (count == 0)
            {
                cmd.CommandText = "update Clients set LastPay = NULL  where IdClient = '" + id.ToString() + "'";
                cmd.ExecuteNonQuery();
            }
            
            if (count != 0)
            {
                DataRowView row = ListPayments.Items.GetItemAt(0) as DataRowView;
                last = DateTime.Parse(row.Row[4].ToString(),  new System.Globalization.CultureInfo("fr"));

                if (count >= 1)
                {
                    DataRowView row1 = ListPayments.Items.GetItemAt(0) as DataRowView;
                    cmd.Parameters.Clear();

                    cmd.CommandText = "update Clients set LastPay = @x  where IdClient = '" + id.ToString() + "'";
                    cmd.Parameters.AddWithValue("@x",DateTime.Parse(row1.Row[4].ToString(), new System.Globalization.CultureInfo("fr")));
                    cmd.ExecuteNonQuery();
                }
            }
            cn.Close();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            loaded();            
        }

        private void ListPayments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListPayments.SelectedIndex != -1)
            {
                ajouter.Content = "Nouveau";
                int index = ListPayments.SelectedIndex;
                DataRowView row = ListPayments.Items.GetItemAt(index) as DataRowView;
                NomTextBox.Text = row.Row[4].ToString();
                PrixTextBox.Text = row.Row[5].ToString();
            }
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext;
            int index = ListPayments.Items.IndexOf(item);
            DataRowView row = ListPayments.Items.GetItemAt(index) as DataRowView;

          
               
            ConfirmForm c = new ConfirmForm("Cette Dépense Sera Définitivement Supprimée, Voulez Vous Vraiment Continuer?");
            c.Owner = this;
            this.Opacity = 0.5;
            this.Effect = new BlurEffect();
            if ((bool)c.ShowDialog())
            {
                cn.Open();
                cmd.Connection = cn;
                cmd.CommandText = "delete from Payments where IdPayment = '" + row.Row[0].ToString() + "'";
                cmd.ExecuteNonQuery();
                cn.Close();

                messageContent.Text = "Paiement Bien Supprimée";
                animateBorder(borderMessage);

                ajouter.Content = "Ajouter";
                NomTextBox.Text = null;
                PrixTextBox.Text = null;
                ListPayments.UnselectAll();
                loaded();

                

            }
            coutItems();
            this.Opacity = 1;
            this.Effect = null;

        }

        private void Modifier_Click(object sender, RoutedEventArgs e)
        {
            if (ListPayments.SelectedIndex == -1)
            {
                messageContent.Text = "Veuillez Sélectionner Une Ligne";
                animateBorder(borderMessage);
            }
            else
            {  if (PrixTextBox.Text == "" || NomTextBox.Text == "")
            {
                messageContent.Text = "Merci De Remplir tous Les Champs";
                animateBorder(borderMessage);
            }
            else
            {
                try
                {
                        DataRowView row1 = ListPayments.Items.GetItemAt(0) as DataRowView;
                       DateTime date = DateTime.Parse(row1.Row[4].ToString(), new System.Globalization.CultureInfo("fr"));


                        int index = ListPayments.SelectedIndex;
                    DataRowView row = ListPayments.Items.GetItemAt(index) as DataRowView;
                    int idpay = int.Parse(row.Row[0].ToString());

                    cn.Open();
                    cmd.Connection = cn;
                        cmd.Parameters.Clear();
                    cmd.CommandText = "update  Payments set date_Payment =@a, Prix ='" +double.Parse(PrixTextBox.Text) + "' where IdPayment ='" + idpay + "'";
                        cmd.Parameters.AddWithValue("@a", DateTime.Parse(NomTextBox.Text.ToString(), new System.Globalization.CultureInfo("fr")));

                        cmd.ExecuteNonQuery();

                    messageContent.Text = "Paiement Bien Modifiée";
                    animateBorder(borderMessage);

                        if (count == 1)
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandText = "update Clients set LastPay = @b  where IdClient = '" + id.ToString() + "'";
                            cmd.Parameters.AddWithValue("@b", DateTime.Parse(NomTextBox.Text.ToString(), new System.Globalization.CultureInfo("fr")));

                            cmd.ExecuteNonQuery();
                        }

                        else if (DateTime.Parse(NomTextBox.Text.ToString(), new System.Globalization.CultureInfo("fr")) > date)
                        {
                            cmd.Parameters.Clear();

                            cmd.CommandText = "update Clients set LastPay = @c  where IdClient = '" + id.ToString() + "'";
                            cmd.Parameters.AddWithValue("@c", DateTime.Parse(NomTextBox.Text.ToString(), new System.Globalization.CultureInfo("fr")));

                            cmd.ExecuteNonQuery();
                        }


                    }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                    MessageForm m = new MessageForm(msg);
                    m.ShowDialog();
                }
                finally
                {
                    cn.Close();
                    ajouter.Content = "Ajouter";
                    NomTextBox.Text = null;
                    PrixTextBox.Text = null;
                    ListPayments.UnselectAll();

                    loaded();
                }
            }

            }
          
            coutItems();

        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            dade.Effect = null;
            dade.Opacity = 1;
            this.Hide();
        }

        private void PrixTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex reg = new Regex(@"\D");
            e.Handled = reg.IsMatch(e.Text);
        }

        DateTime date;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ajouter.Content.ToString() == "Nouveau")
            {
                ajouter.Content = "Ajouter";
                NomTextBox.Text = null;
                PrixTextBox.Text = null;
                ListPayments.UnselectAll();
            }
            else if (ajouter.Content.ToString() == "Ajouter")
            {
                if (PrixTextBox.Text=="" || NomTextBox.Text=="")
                {
                    messageContent.Text = "Merci De Remplir tous Les Champs";
                    animateBorder(borderMessage);
                }
                else
                {
                    try
                    {
                        if(ListPayments.SelectedIndex != -1)
                        {
                            DataRowView row1 = ListPayments.Items.GetItemAt(0) as DataRowView;
                             date = DateTime.Parse(row1.Row[4].ToString(), new System.Globalization.CultureInfo("fr"));
                        }
                        

                        cn.Open();
                            cmd.Connection = cn;
                        cmd.Parameters.Clear();
                            cmd.CommandText = "insert into Payments values (@a,'" + id.ToString() + "','" + ConnectedSalle.ToString() + "','" + ConnectedSport.ToString() + "','" +double.Parse(PrixTextBox.Text) + "')";
                        cmd.Parameters.AddWithValue("@a", DateTime.Parse(NomTextBox.Text.ToString(), new System.Globalization.CultureInfo("fr")));

                        cmd.ExecuteNonQuery();


                        messageContent.Text = "Paiement Bien Ajoutée";
                        animateBorder(borderMessage);

                        if (DateTime.Parse(NomTextBox.Text.ToString(), new System.Globalization.CultureInfo("fr")) > date)
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandText = "update Clients set LastPay = @b  where IdClient = '" + id.ToString() + "'";
                            cmd.Parameters.AddWithValue("@b", DateTime.Parse(NomTextBox.Text.ToString(), new System.Globalization.CultureInfo("fr")));
                            
                            cmd.ExecuteNonQuery();
                        }                     
                           
                    }
                    catch (Exception ex)
                    {
                        string msg = ex.Message;
                        MessageForm m = new MessageForm(msg);
                        m.ShowDialog();
                    }
                    finally
                    {
                        cn.Close();
                        loaded();
                        NomTextBox.Text = null;
                        PrixTextBox.Text = null;
                    }
                }
               
            }

        }

        public void animateBorder(Border c)
        {
            ((Storyboard)GridGlobal.Resources["animate"]).Begin(c);
        }
    }
}
