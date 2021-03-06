﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Arma3BEClient.Models;
using Arma3BEClient.Updater.Models;
using Arma3BEClient.ViewModel;

namespace Arma3BEClient.Chat
{
    /// <summary>
    /// Interaction logic for ChatControl.xaml
    /// </summary>
    public partial class ChatControl : UserControl{
        


        private ServerMonitorChatViewModel Model { get { return DataContext as ServerMonitorChatViewModel; } }

        public ChatControl()
        {
            InitializeComponent();
            InitBox();
        }

        void _model_ChatMessageEventHandler(object sender, Updater.Models.ChatMessage e)
        {
            Dispatcher.Invoke(() =>
            {
                if (!Model.EnableChat) return;
                var type = e.Type;
                if (type != ChatMessage.MessageType.Unknown)
                    AppendText(paragraph, ChatScrollViewer, e);
                else
                    AppendText(msgConsole, ConsoleScrollViewer, e);
            });
        }

        private void SendMessage(object sender, RoutedEventArgs e)
        {
            Model.SendMessage();
        }


        private void KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var text = tbNewMessage.Text;
                Model.SendMessage(text);
            }
        }


        private Paragraph paragraph;

        private void InitBox()
        {
            msgBox.Document.Blocks.Clear();
            paragraph = new Paragraph();

            msgBox.Document.Blocks.Add(paragraph);
        }


        public void AppendText(Paragraph p, ScrollViewer scroll, ChatMessage message)
        {
            var text = string.Format("[ {0:HH:mm:ss} ]  {1}\n", message.Date, message.Message);
            var color = ServerMonitorModel.GetMessageColor(message);
            
            var brush = new SolidColorBrush(color);
            var span = new Span() { Foreground = brush };
            span.Inlines.Add(text);
            paragraph.Inlines.Add(span);

            if (Model.AutoScroll)
                scroll.ScrollToEnd();
        }


        public void AppendText(TextBox block, ScrollViewer scroll, ChatMessage message)
        {
            var text = string.Format("[ {0:HH:mm:ss} ]  {1}\n", message.Date, message.Message);
            block.Text += text;
            
            if (Model.AutoScroll)
            scroll.ScrollToEnd();
        }

        private void ToolBar_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Model.ChatMessageEventHandler += _model_ChatMessageEventHandler;
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            msgBox.Document.Blocks.Clear();
            msgConsole.Text = string.Empty;

            InitBox();

            msgBox.AppendText(Environment.NewLine);
        }
    }
}
