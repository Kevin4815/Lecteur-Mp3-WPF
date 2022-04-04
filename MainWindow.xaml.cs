using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Timers;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace MP3_WPF_2022
{
    public partial class MainWindow : Window
    {
        Timer m_TimerTest;
        MediaPlayer m_MediaPlayer = new MediaPlayer();
        TagLib.File m_Title;
        List<string> m_Song = new List<string>();

        public bool m_Play = false;
        public bool m_FirstStart = true;
        public bool m_Forward = false;
        public bool m_ClikOnList = false;
        public bool m_AddList = false;
        public bool m_TestList = false;

        double m_SliderSecondes;
        int i = 0;
        int index = 0;

        string m_FolderPath;
        string titleName;
        string m_CurPosition;
        string m_Duration;
        string m_Position;
        string m_CutDuration;
        string m_Location = @"C:\Users\kevin\Desktop\Projet portfolio\MP3-WPF-2022\Images/";

        string[] m_Files;

        public MainWindow()
        {
            Start();
            Timer(10, CheckVolume_Tick, new TimeSpan(0,0,0,0,1));
            slider.Visibility = Visibility.Hidden;
        }

        void fileExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Close this window
            this.Close();
        }

        private void GetTimePosition_Tick(object sender, EventArgs e)
        {
            if(m_AddList == true)
            {
                info.Content = m_Song.Count + " titre disponible";
                slider.Visibility = Visibility.Visible;
                info.HorizontalAlignment = HorizontalAlignment.Center;

                if (m_Play == true)
                {
                    //Position of the song in real time
                    m_Position = m_MediaPlayer.Position.ToString();
                    m_CurPosition = m_Position.Substring(0, 8);
                    labelElapsed.Content = m_CurPosition;

                    //Check the total value of the current music
                    UpdateDurationValue();

                    //Update slider position
                    UpdatePositionInTheSong();

                    //Next music if finished
                    if (slider.Value >= 100)
                    {
                        i++;
                        UpdateSongValue();
                    }

                    //Forcing the CommandManager to raise the RequerySuggested event
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        private void slider_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //For move in time in the music to move forward or backward
            UpdatePositionInTheSong();
            m_MediaPlayer.Position = TimeSpan.FromSeconds(m_SliderSecondes);
        }

        private void CheckVolume_Tick(object sender, EventArgs e)
        {
            //Adjust the volume of music with a slider
            m_MediaPlayer.Volume = volume.Value;

            if (volume.Value > 0)
            {
                sound.Source = new BitmapImage(new Uri(m_Location + "sound.png", UriKind.Absolute));
            }
            else
            {
                sound.Source = new BitmapImage(new Uri(m_Location + "mute.png", UriKind.Absolute));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            m_Play = !m_Play;

            if (m_AddList == true)
            {
                m_Title = TagLib.File.Create(m_Song[i]);

                if (m_Play == true)
                {
                    btnPlay.Source = new BitmapImage(new Uri(m_Location + "pause.png", UriKind.Absolute));
                    m_MediaPlayer.Play();

                    //Recover the name of the artist and the title of the current song
                    title.Text = m_Title.Tag.Title;
                    artist.Text = m_Title.Tag.FirstPerformer;
                    UpdateDurationValue();
                }
                else
                {
                    //Music  on pause
                    m_TimerTest.Stop();
                    btnPlay.Source = new BitmapImage(new Uri(m_Location + "bouton-jouer.png", UriKind.Absolute));
                    m_MediaPlayer.Pause();
                    m_FirstStart = false;
                }
            }
            else
            {
                title.Text = "Veuillez ajouter une playlist";
   
                m_Play = false;

            }
        }

        private void Timer(int interval, EventHandler e, TimeSpan time)
        {
            m_TimerTest = new Timer()
            {
                Interval = 1000
            };

            var dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += e;
            dispatcherTimer.Interval = time;
            dispatcherTimer.Start();
        }

        private void Button_Next_Click(object sender, RoutedEventArgs e)
        {
            //Go to the next music and update the duration and cursor position
            if(m_AddList == true)
            {
                if (m_Play == true)
                {
                    if(i >= m_Song.Count -1)
                        i = m_Song.Count -1;
                    else
                        i++;

                    UpdateSongValue();
                }
            }
        }

        private void Button_Previous_Click(object sender, RoutedEventArgs e)
        {
            //Go to the previous music and update the duration and cursor position
            if(m_AddList == true)
            {
                if(m_Play == true)
                {
                    if (i <= 0)
                        i = 0;
                    else
                        i--;

                    UpdateSongValue();
                }
            }
        }

        private void UpdateDurationValue()
        {
            //Duration of the song
            m_Duration = m_MediaPlayer.NaturalDuration.ToString();
            m_CutDuration = m_Duration.Substring(0, 8);
            labelDuration.Content = m_CutDuration;
        }

        private void InstantiateTitleButton()
        {
            string a = @"\";

            for (int i = 0; i < m_Song.Count; i++)
            {
                string empty = m_Song[i].Replace(" ", "");

                for (int j = 0; j < empty.Length; j++)
                {
                    if(empty[j] == char.Parse(a))
                    {
                        index = j;
                    }
                }

                Button b = new Button();
                b.Content = empty.Substring(index +1);
                b.HorizontalAlignment = HorizontalAlignment.Left;
                b.Background = new SolidColorBrush(Colors.White);
                b.BorderBrush = new SolidColorBrush(Colors.White);
                b.Click += ChangeTitleOnClick;
                b.Tag = i;
                Grid.SetRow(b, i);
                grid.Children.Add(b);
            }
        }

        private void ChangeTitleOnClick(object sender ,RoutedEventArgs e)
        {
            //The value of "i" becomes the index of clicked button
            object tag = ((Button)sender).Tag;
            i = (int)tag;
            m_Title = TagLib.File.Create(m_Song[i]);
            m_ClikOnList = true;
            titleName = (sender as Button).Content.ToString();
            m_MediaPlayer.Open(new Uri(m_Files[i], UriKind.Relative));
            slider.Value = 0;

            if (m_Play == true)
            {
                UpdateSongValue();
            }
        }

        private void UpdateSongValue()
        {
            m_MediaPlayer.Open(new Uri(m_Song[i], UriKind.Relative));
            m_MediaPlayer.Play();
            slider.Value = 0;
            m_Title = TagLib.File.Create(m_Song[i]);

            //Recover the name of the artiste and the title of the current music
            title.Text = m_Title.Tag.Title;
            artist.Text = m_Title.Tag.FirstPerformer;

            //Update the duration of the current music
            System.Threading.Thread.Sleep(500);
            UpdateDurationValue();
        }

        private void UpdatePositionInTheSong()
        {
            //Number between each tick
            double sliderEcart = 1f / m_MediaPlayer.NaturalDuration.TimeSpan.TotalSeconds * 100;

            //Allows the cursor to keep its new position once it moves
            m_SliderSecondes = slider.Value / sliderEcart;

            slider.Value = m_SliderSecondes / m_MediaPlayer.NaturalDuration.TimeSpan.TotalSeconds * 100; // current position / total duration
            slider.Value += sliderEcart;
        }

        private void AddTitleInList()
        {
            if(m_AddList == true)
            {
                title.Text = "Prêt à l'écoute";

                //Add each songs to the list
                foreach (string file in m_Files)
                {
                    m_Song.Add(file);
                    grid.RowDefinitions.Add(new RowDefinition());
                }
            }
        }

        //Load the folder of musics
        private void AddMusicFolder_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;

            CommonFileDialogResult result = dialog.ShowDialog();

            //If the window has been opened and closed without add musics
            try
            {
                m_FolderPath = dialog.FileName;
                m_TestList = true;
                Start();
            }
            catch
            {
                if(m_TestList == false)
                {
                    title.Text = "Veuillez ajouter une playlist";
                }
            }
        }

        //Once the folder has been added, the music can start
        public void Start()
        {
            InitializeComponent();
            volume.Value = m_MediaPlayer.Volume;

            if (m_TestList == true)
            {
                m_Files = Directory.GetFiles(m_FolderPath);
                TestExtensionFormat();
                AddTitleInList();

                if (m_AddList == true)
                {
                    //Change the value of the slider
                    Timer(1000, GetTimePosition_Tick, new TimeSpan(0,0,1));
                }

                //Choose the first title to read
                m_MediaPlayer.Open(new Uri(m_Song[i], UriKind.Relative));
                InstantiateTitleButton();
            }
        }

        private void TestExtensionFormat() 
        {
            //Check if the contents of the folder are to the correct format
            for (int i = 0; i < m_Files.Length; i++)
            {
                if (m_Files[i].Contains(".mp3"))
                {
                    m_AddList = true;
                }
                else
                {
                    m_AddList = false;
                    title.Text = "Erreur, format non valide";
                }
            }
        }
    }
}
