using Prism.Mvvm;
using Reactive.Bindings;

namespace SocketCommand8000.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {

        public ReactiveCommand Open { get; }
        public ReactiveCommand Close { get; }
        public ReactiveCommand Connect { get; }
        public ReactiveCommand Send { get; }

        public MainWindowViewModel()
        {
            Models.MyTcpListener listener = new Models.MyTcpListener("127.0.0.1", 10000);
            Models.MyTcpClient client = new Models.MyTcpClient("127.0.0.1", 10000);

            Open = new ReactiveCommand()
                .WithSubscribe(() =>
                {
                    listener.Start();
                });
            
            Close = new ReactiveCommand()
                .WithSubscribe(() =>
                {
                    listener.Stop();
                });

            Connect = new ReactiveCommand()
                .WithSubscribe(() =>
                {
                    //client.Connect();
                });

            Send = new ReactiveCommand()
                .WithSubscribe(() =>
                {
                    byte[] message = new byte[] {0x06, 0x00, 0x01, 0x02, 0x03, 0x04 };
                    client.Send(message);
                });

        }
    }
}
