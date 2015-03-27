using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace BackKeyDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private UIStateMachine stateMachine;
        private Stack<IBackable> _controlBackStack = new Stack<IBackable>();
        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            stateMachine = new UIStateMachine();
            stateMachine.BackStack = _controlBackStack;
            
            this.DataContext = stateMachine;
            Loaded += MainPage_Loaded;
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("MainPage BackPressed added");
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            stateMachine.BackStack.Push(stateMachine);
        }


        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            stateMachine.BackStack.Pop();
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Debug.WriteLine("MainPage BackPressed triggered");
            if (!e.Handled && _controlBackStack != null && _controlBackStack.Count > 0)
            {
                e.Handled = _controlBackStack.Peek().GoBack();
            }
            else { 
                // <application specific> post processing when GoBack failed in the current control
            }
        }
         

        private void Button_Click_PlusOne(object sender, RoutedEventArgs e)
        {
            stateMachine.Perform(Command.PlusOne);
        }

        private void Button_Click_PlusTwo(object sender, RoutedEventArgs e)
        {
            stateMachine.Perform(Command.PlusTwo);
        }

        private void Button_Click_MinusOne(object sender, RoutedEventArgs e)
        {
            stateMachine.Perform(Command.MinusOne);
        }

        private void Button_Click_MinusTwo(object sender, RoutedEventArgs e)
        {
            stateMachine.Perform(Command.MinusTwo);
        }

        private void Option_Click_Reset(object sender, RoutedEventArgs e)
        {
            stateMachine.Perform(Command.Reset);
        }

        private void Option_Click_About(object sender, RoutedEventArgs e)
        {
            ShowAbout();
        }

        private async void ShowAbout() {
            //first line
            TextBlock line1 = new TextBlock();
            line1.Text = "This app is to demo";
            line1.FontSize = 17;
            line1.Foreground = new SolidColorBrush(Colors.White);
            line1.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;

            //second line
            TextBlock line2 = new TextBlock();
            line2.Text = "how back button impacts the UI flow.";
            line2.FontSize = 17;
            line2.Foreground = new SolidColorBrush(Colors.White);
            line2.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;
            
            StackPanel stack = new StackPanel();
            stack.Children.Add(line1);
            stack.Children.Add(line2);

            ContentDialog dialog = new ContentDialog();
            dialog.Content = stack;
            SolidColorBrush color = new SolidColorBrush(Colors.Black);
            color.Opacity = 0.8;
            dialog.Background = color;
            dialog.Margin = new Thickness(0, 250, 0, 0);
            await dialog.ShowAsync();
        }
    }
}
