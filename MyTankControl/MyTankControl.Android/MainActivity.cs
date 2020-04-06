using System;
using System.Threading;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

//added for bluotooth
using System.IO;
using Java.Util;
using Android.Bluetooth;
using System.Threading.Tasks;


namespace MyTankControl.Droid
{
    [Activity(Label = "MyTankControl", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        //Creamos las variables necesarios para trabajar
        //Widgets
        Xamarin.Forms.Label labelMotorP;
        Xamarin.Forms.Label labelESPP;
        Xamarin.Forms.Label labelInfo;
        Xamarin.Forms.Button ButtonConnect;
        Xamarin.Forms.Button ButtonLF;
        Xamarin.Forms.Button ButtonLS;
        Xamarin.Forms.Button ButtonLB;
        Xamarin.Forms.Button ButtonRF;
        Xamarin.Forms.Button ButtonRS;
        Xamarin.Forms.Button ButtonRB;

        bool LF = false;
        bool LS = false;
        bool LB = false;
        bool RF = false;
        bool RS = false;
        bool RB = false;

        //String a enviar
        private Java.Lang.String dataToSend;
        //Variables para el manejo del bluetooth Adaptador y Socket
        private BluetoothAdapter mBluetoothAdapter = null;
        private BluetoothSocket btSocket = null;
        //Streams de lectura I/O
        private Stream outStream = null;
        private Stream inStream = null;
        //MAC Address del dispositivo Bluetooth
        private static string address = "24:0A:C4:32:1D:0A";//MyTank
        //private static string address = "24:0A:C4:32:1A:0E";//MyTest
        //Id Unico de comunicacion
        private static UUID MY_UUID = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            var app = new App();
            LoadApplication(app);

            //Assign controls and add handlers
            labelInfo = (Xamarin.Forms.Label)app.MainPage.FindByName("labelInfo");
            labelMotorP  = (Xamarin.Forms.Label)app.MainPage.FindByName("labelMotorP");
            labelESPP = (Xamarin.Forms.Label)app.MainPage.FindByName("labelESPP");
            ButtonConnect = (Xamarin.Forms.Button)app.MainPage.FindByName("ButtonConnect");
            ButtonLF = (Xamarin.Forms.Button)app.MainPage.FindByName("ButtonLF");
            ButtonLS = (Xamarin.Forms.Button)app.MainPage.FindByName("ButtonLS");
            ButtonLB = (Xamarin.Forms.Button)app.MainPage.FindByName("ButtonLB");
            ButtonRF = (Xamarin.Forms.Button)app.MainPage.FindByName("ButtonRF");
            ButtonRS = (Xamarin.Forms.Button)app.MainPage.FindByName("ButtonRS");
            ButtonRB = (Xamarin.Forms.Button)app.MainPage.FindByName("ButtonRB");

            ButtonConnect.Clicked += ButtonConnect_CLicked;
            ButtonLF.Clicked += ButtonLF_CLicked;
            ButtonLS.Clicked += ButtonLS_CLicked;
            ButtonLB.Clicked += ButtonLB_CLicked;
            ButtonRF.Clicked += ButtonRF_CLicked;
            ButtonRS.Clicked += ButtonRS_CLicked;
            ButtonRB.Clicked += ButtonRB_CLicked;
            

            CheckBt();
        }
       
        private void CheckBt()
        {
            mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            
            if(mBluetoothAdapter != null && mBluetoothAdapter.Enable())
                Connect("c");
        }
        private void ButtonConnect_CLicked(object sender, EventArgs e)
        {
            //try to close socket
            try
            {
                btSocket.Close();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            //reconnect
            try
            {
                CheckBt();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void ButtonLF_CLicked(object sender, EventArgs e)
        {
            //Toast.MakeText(this, "LF",
                   // ToastLength.Short).Show();
            // throw new NotImplementedException();
            if (mBluetoothAdapter == null || LF) return;

            LF = true;
            LS = false;
            LB = false;

            dataToSend = new Java.Lang.String("0");
            writeData(dataToSend);

            /*
            try
            {
                btSocket.Close();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }*/

        }
        private void ButtonLS_CLicked(object sender, EventArgs e)
        {
           // Toast.MakeText(this, "LS",
                   // ToastLength.Short).Show();
            // throw new NotImplementedException();
            if (mBluetoothAdapter == null || LS) return;

            LF = false;
            LS = true;
            LB = false;

            dataToSend = new Java.Lang.String("1");
            writeData(dataToSend);

           
        }
        private void ButtonLB_CLicked(object sender, EventArgs e)
        {
           // Toast.MakeText(this, "LB",
                  //  ToastLength.Short).Show();
            // throw new NotImplementedException();

            if (mBluetoothAdapter == null || LB) return;

            LF = false;
            LS = false;
            LB = true;

            dataToSend = new Java.Lang.String("2");
            writeData(dataToSend);
        }
        private void ButtonRF_CLicked(object sender, EventArgs e)
        {
            //Toast.MakeText(this, "RF",
                   // ToastLength.Short).Show();
            // throw new NotImplementedException();

            if (mBluetoothAdapter == null || RF) return;

            RF = true;
            RS = false;
            RB = false;

            dataToSend = new Java.Lang.String("3");
            writeData(dataToSend);
        }
        private void ButtonRS_CLicked(object sender, EventArgs e)
        {
           // Toast.MakeText(this, "RS",
                  //  ToastLength.Short).Show();
            // throw new NotImplementedException();

            if (mBluetoothAdapter == null || RS) return;

            RF = false;
            RS = true;
            RB = false;

            dataToSend = new Java.Lang.String("4");
            writeData(dataToSend);
        }
        private void ButtonRB_CLicked(object sender, EventArgs e)
        {
           // Toast.MakeText(this, "RB",
                //    ToastLength.Short).Show();
            // throw new NotImplementedException();

            if (mBluetoothAdapter == null || RB) return;

            RF = false;
            RS = false;
            RB = true;

            dataToSend = new Java.Lang.String("5");
            writeData(dataToSend);
        }
        void tgConnect_HandleCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (e.IsChecked)
            {
                //si se activa el toggle button se incial el metodo de conexion
                Connect("n");
            }
            else
            {
                //en caso de desactivar el toggle button se desconecta del arduino
                if (btSocket.IsConnected)
                {
                    try
                    {
                        btSocket.Close();
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }
        //Evento de conexion al Bluetooth
        public void Connect(string str)
        {            
            //Iniciamos la conexion con el arduino
            BluetoothDevice device = mBluetoothAdapter.GetRemoteDevice(address);
            System.Console.WriteLine("Connecting to " + device);

            //Indicamos al adaptador que ya no sea visible
            mBluetoothAdapter.CancelDiscovery();
            try
            {
                //Inicamos el socket de comunicacion con el arduino
                btSocket = device.CreateRfcommSocketToServiceRecord(MY_UUID);
                //Conectamos el socket
                btSocket.Connect();
                System.Console.WriteLine("Connected");
                //labelInfo.Text = "Connected";
            }
            catch (System.Exception e)
            {
                //en caso de generarnos error cerramos el socket
                Console.WriteLine(e.Message);
                try
                {
                    btSocket.Close();
                }
                catch (System.Exception)
                {
                    System.Console.WriteLine("Impossible connection");
                }
                System.Console.WriteLine("Socket created!");
                //labelInfo.Text = "Disconnected";
            }
            //Una vez conectados al bluetooth mandamos llamar el metodo que generara el hilo
            //que recibira los datos del arduino
            beginListenForData();
            //NOTA envio la letra e ya que el sketch esta configurado para funcionar cuando
            //recibe esta letra.
            dataToSend = new Java.Lang.String(str);
            writeData(dataToSend);
        }
        //Evento para inicializar el hilo que escuchara las peticiones del bluetooth
        public void beginListenForData()
        {
            //Extraemos el stream de entrada
            try
            {
                inStream = btSocket.InputStream;
            }
            catch (System.IO.IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
            //Creamos un hilo que estara corriendo en background el cual verificara si hay algun dato
            //por parte del arduino
            var t = Task.Factory.StartNew(() => {
                //declaramos el buffer donde guardaremos la lectura
                byte[] buffer = new byte[1024];
                //declaramos el numero de bytes recibidos
                int bytes;
                while (true)
                {
                    try
                    {
                        //leemos el buffer de entrada y asignamos la cantidad de bytes entrantes
                        bytes = inStream.Read(buffer, 0, buffer.Length);
                        //Verificamos que los bytes contengan informacion
                        if (bytes > 0)
                        {
                            //Corremos en la interfaz principal
                            RunOnUiThread(() => {
                                //Convertimos el valor de la informacion llegada a string
                                string valor = System.Text.Encoding.ASCII.GetString(buffer);
                                //Agregamos a nuestro label la informacion llegada
                                string[] vals = valor.Split(new string[] { "|" }, StringSplitOptions.None);

                                if (vals.Length == 3)
                                {
                                    Thread.Sleep(10);
                                    labelMotorP.Text = "Motor:" + vals[1];
                                    Thread.Sleep(10);
                                    labelESPP.Text = "ESP:" + vals[2];
                                }
                            });
                        }
                    }
                    catch (Java.IO.IOException)
                    {
                        //En caso de error limpiamos nuestra label y cortamos el hilo de comunicacion
                        RunOnUiThread(() => {
                           // labelMotorP.Text = "Motor: -----";
                            //labelESPP.Text = "ESP: -----";
                        });
                        break;
                    }
                    Thread.Sleep(100);
                }
            });
        }
        //Metodo de envio de datos la bluetooth
        private void writeData(Java.Lang.String data)
        {
            //Extraemos el stream de salida
            try
            {
                outStream = btSocket.OutputStream;
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Error with sending: " + e.Message);
            }

            //creamos el string que enviaremos
            Java.Lang.String message = data;

            //lo convertimos en bytes
            byte[] msgBuffer = message.GetBytes();

            try
            {
                //Escribimos en el buffer el arreglo que acabamos de generar
                outStream.Write(msgBuffer, 0, msgBuffer.Length);
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Error with sending: " + e.Message);
            }
        }
    }
    
}