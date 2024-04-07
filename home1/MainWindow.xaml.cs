using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Threading;
namespace _02_TaskManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer = null;
        int[] speed = { 1, 2, 5 };
        int index = 0;
        public MainWindow()
        {
            InitializeComponent();
            Refresh();
            timer = new DispatcherTimer();
            timer.Interval=new TimeSpan(0,0,2);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private void Refresh()
        {
            grid.ItemsSource = Process.GetProcesses();
            
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            //MessageBox.Show("Time");
            Refresh();
        }

        private void Details(object sender, RoutedEventArgs e)
        {
            var res = grid.SelectedItem as Process;
          
            if (res == null) { return; }
            string info;
            try
            {
                info = $"Id :: {res.Id}; {res.ProcessName} {res.StartTime}";
            }
            catch(Exception ex)
            {
                info = $"Error :: Id :: {res.Id}; {res.ProcessName}";

            }
            MessageBox.Show(info);
        }

        private void Stop(object sender, RoutedEventArgs e)
        {
            //MessageBoxResult messageBoxResult = MessageBox.Show(speed[index.ToString()]);
            var res = grid.ItemsSource as Process;
            if (res == null) { return; }
            res.Kill();
        }


        private void NextSpeed(object sender, RoutedEventArgs e)
        {
            
            index++;
            if(index==speed.Length)
            {
                index = 0;
            }
            timer.Interval = new TimeSpan(0, 0, speed[index]);
        }
    }
}