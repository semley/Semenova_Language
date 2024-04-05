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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Semenova_Language
{
    /// <summary>
    /// Логика взаимодействия для ClientPage.xaml
    /// </summary>
    public partial class ClientPage : Page
    {
        int CountInPage = 10;
        int CountRecords;
        int CountPage;
        int CurrentPage = 0;

        List<Client> CurrentPageList = new List<Client>();
        List<Client> TableList;
        public ClientPage()
        {
            InitializeComponent();
            //var currentClient = Семенова_LanguageEntities.GetContext().Client.ToList();
            //ServiceListView.ItemsSource = currentClient;
            List<Client> currentClients = Семенова_LanguageEntities.GetContext().Client.ToList();

            ServiceListView.ItemsSource = currentClients;
            //FiltrBox.SelectedIndex = 0;
            //strCount.SelectedIndex = 0;
            //SortBox.SelectedIndex = 0;
            TBAllRecords.Text = Семенова_LanguageEntities.GetContext().Client.ToList().Count().ToString();
            Update();
        }


        public void Update()
        {
            var currentClient = Семенова_LanguageEntities.GetContext().Client.ToList();
            
            if (SortBox.SelectedIndex == 1)
            {
                currentClient = currentClient.OrderBy(p => p.FirstName).ToList();
            }
            else if (SortBox.SelectedIndex == 2)
            {
                currentClient = currentClient.OrderByDescending(p => DateTime.TryParse(p.LastVisitDate, out DateTime visitDate) ? visitDate : DateTime.MinValue).ToList();
            }
            else if (SortBox.SelectedIndex == 3)
            {
                currentClient = currentClient.OrderByDescending(p => p.VisitCount).ToList();
            }

            if (FiltrBox.SelectedIndex == 1)
            {
                currentClient = currentClient.Where(p => p.GenderCode == "ж").ToList();
            }
            else if (FiltrBox.SelectedIndex == 2)
            {
                currentClient = currentClient.Where(p => p.GenderCode == "м").ToList();
            }

            currentClient = currentClient.Where(p => p.LastName.ToLower().Contains(TBoxSearch.Text.ToLower()) || p.FirstName.ToLower().Contains(TBoxSearch.Text.ToLower()) || p.Patronymic.ToLower().Contains(TBoxSearch.Text.ToLower()) || p.Email.ToLower().Contains(TBoxSearch.Text.ToLower()) || p.Phone.Replace("+", "").Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "").ToLower().Contains(TBoxSearch.Text.Replace("+", "").Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "").ToLower())).ToList();
            
            TBAllRecords.Text = Семенова_LanguageEntities.GetContext().Client.ToList().Count().ToString();
            TBCount.Text = currentClient.Count().ToString();

            ServiceListView.ItemsSource = currentClient;

            TableList = currentClient;

            if (strCount.SelectedIndex == 0)
            {
                CountInPage = 10;
            }
            else if (strCount.SelectedIndex == 1)
            {
                CountInPage = 50;
            }
            else if (strCount.SelectedIndex == 2)
            {
                CountInPage = 200;
            }
            else if (strCount.SelectedIndex == 3)
            {
                CountInPage = 0;
            }


            ChangePage(0, 0);
        }

        private void ChangePage(int direction, int? selectedPage)
        {
            CurrentPageList.Clear();
            CountRecords = TableList.Count;
            if (CountInPage != 0)
            {
                if (CountRecords % CountInPage > 0)
                {
                    CountPage = CountRecords / CountInPage + 1;
                }
                else
                {
                    CountPage = CountRecords / CountInPage;
                }

                Boolean Ifupdate = true;

                int min;

                if (selectedPage.HasValue)
                {
                    if (selectedPage >= 0 && selectedPage <= CountPage)
                    {
                        CurrentPage = (int)selectedPage;
                        min = CurrentPage * CountInPage + CountInPage < CountRecords ? CurrentPage * CountInPage + CountInPage : CountRecords;
                        for (int i = CurrentPage * CountInPage; i < min; i++)
                        {
                            CurrentPageList.Add(TableList[i]);
                        }
                    }
                }
                else
                {
                    switch (direction)
                    {
                        case 1:
                            if (CurrentPage > 0)
                            {
                                CurrentPage--;
                                min = CurrentPage * CountInPage + CountInPage < CountRecords ? CurrentPage * CountInPage + CountInPage : CountRecords;
                                for (int i = CurrentPage * CountInPage; i < min; i++)
                                {
                                    CurrentPageList.Add(TableList[i]);
                                }
                            }
                            else
                            {
                                Ifupdate = false;
                            }
                            break;
                        case 2:
                            if (CurrentPage < CountPage - 1)
                            {
                                CurrentPage++;
                                min = CurrentPage * CountInPage + CountInPage < CountRecords ? CurrentPage * CountInPage + CountInPage : CountRecords;
                                for (int i = CurrentPage * CountInPage; i < min; i++)
                                {
                                    CurrentPageList.Add(TableList[i]);
                                }
                            }
                            else
                            {
                                Ifupdate = false;
                            }
                            break;
                    }
                }
                if (Ifupdate)
                {
                    PageListBox.Items.Clear();
                    for (int i = 1; i <= CountPage; i++)
                    {
                        PageListBox.Items.Add(i);
                    }
                    PageListBox.SelectedIndex = CurrentPage;

                    //min = CurrentPage * CountInPage + CountInPage < CountRecords ? CurrentPage * CountInPage + CountInPage : CountRecords;
                    //TBCount.Text = min.ToString();
                    //TBAllRecords.Text = CountRecords.ToString();

                    ServiceListView.ItemsSource = CurrentPageList;

                    ServiceListView.Items.Refresh();
                }
            }
            else
            {
                PageListBox.Items.Clear();
                PageListBox.Items.Add(1);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var currentClient = (sender as Button).DataContext as Client;

            if (currentClient.VisitCount == 0)
            {
                if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Семенова_LanguageEntities.GetContext().Client.Remove(currentClient);
                    Семенова_LanguageEntities.GetContext().SaveChanges();
                    ServiceListView.ItemsSource = Семенова_LanguageEntities.GetContext().Client.ToList();
                    Update();
                }
            }
            else
            {
                MessageBox.Show("Невозможно выполнить удаление, так как у клиента есть посещения!");
            }
        }

        private void strCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Update();
        }

        private void LeftDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(1, null);
        }

        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangePage(0, Convert.ToInt32(PageListBox.SelectedItem.ToString()) - 1);
        }

        private void RightDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(2, null);
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Update();
        }

        private void FiltrBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Update();
        }

        private void SortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Update();
        }
    }
}
