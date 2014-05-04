using System.Windows;
using SharpBlade;
using SharpBlade.Native;
using SharpBlade.Razer;
using SharpBlade.Razer.Events;
using Sharparam.Lib;
using System.Collections.Generic;
using System.Windows.Shapes;
using System;
using Midi;
using System.Threading;

namespace SharpBlade.Piano
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RazerManager _razer;
        private readonly OverlayHelper _overlayHelper;
        private OutputDevice _outputDevice = null;
        private Dictionary<Rectangle, Pitch> _notes;
        private Pitch _currentPitch;
        public MainWindow()
        {
            //Init XAML
            InitializeComponent();
            //Create note list
            _razer = Provider.Razer;
            _notes = new Dictionary<Rectangle,Pitch>();
            _notes.Add(NoteCS,Pitch.CSharp4);
            _notes.Add(NoteDS,Pitch.DSharp4);
            _notes.Add(NoteFS,Pitch.FSharp4);
            _notes.Add(NoteGS,Pitch.GSharp4);
            _notes.Add(NoteAS,Pitch.ASharp4);
            _notes.Add(NoteC,Pitch.C4);
            _notes.Add(NoteD,Pitch.D4);
            _notes.Add(NoteE,Pitch.E4);
            _notes.Add(NoteF,Pitch.F4);
            _notes.Add(NoteG,Pitch.G4);
            _notes.Add(NoteA,Pitch.A4);
            _notes.Add(NoteB,Pitch.B4);

            //sound device
            if (OutputDevice.InstalledDevices.Count > 0)
            {
                _outputDevice = OutputDevice.InstalledDevices[0];
                _outputDevice.Open();
                _outputDevice.SendProgramChange(Channel.Channel1, Instrument.AcousticGrandPiano);
            }
            else
                Application.Current.Shutdown();

            _overlayHelper = new OverlayHelper(NewMessageOverlay, NewMessageOverlayLabel);
            

            /* This sends the current window to the SBUI
             * We give it the Polling RenderMethod which updates
             * SBUI every 42ms (about 24FPS)
             */
            _razer.Touchpad.SetWindow(this, Touchpad.RenderMethod.Polling);

            _razer.EnableDynamicKey(RazerAPI.DynamicKeyType.DK1, OnPressPiano, @"Default\Images\Piano.png");
            _razer.EnableDynamicKey(RazerAPI.DynamicKeyType.DK2, OnPressGuitar, @"Default\Images\Guitar.png");
            _razer.EnableDynamicKey(RazerAPI.DynamicKeyType.DK3, OnPressEPiano, @"Default\Images\EPiano.png");
            _razer.EnableDynamicKey(RazerAPI.DynamicKeyType.DK4, OnPressXylophone, @"Default\Images\Xylo.png");
            _razer.EnableDynamicKey(RazerAPI.DynamicKeyType.DK5, OnPressSteelDrums, @"Default\Images\Drums.png");
            _razer.Touchpad.Gesture += TouchpadOnGesture;
            //_razer.Touchpad.EnableGesture(RazerAPI.GestureType.Press);
            _razer.Touchpad.EnableGesture(RazerAPI.GestureType.Move);
            _razer.Touchpad.EnableGesture(RazerAPI.GestureType.Tap);
            //_overlayHelper.Show("Ready to touch");

        }

        private void OnPressPiano(object sender, System.EventArgs e)
        {
            _outputDevice.SendProgramChange(Channel.Channel1, Instrument.AcousticGrandPiano);
        }
        private void OnPressGuitar(object sender, System.EventArgs e)
        {
            _outputDevice.SendProgramChange(Channel.Channel1, Instrument.AcousticGuitarSteel);
        }
        private void OnPressEPiano(object sender, System.EventArgs e)
        {
            _outputDevice.SendProgramChange(Channel.Channel1, Instrument.ElectricPiano1);
        }
        private void OnPressXylophone(object sender, System.EventArgs e)
        {
            _outputDevice.SendProgramChange(Channel.Channel1, Instrument.Xylophone);
        }
        private void OnPressSteelDrums(object sender, System.EventArgs e)
        {
            _outputDevice.SendProgramChange(Channel.Channel1, Instrument.SteelDrums);
        }

        private void TouchpadOnGesture(object sender, GestureEventArgs gestureEventArgs)
        {
            var xPos = gestureEventArgs.X;
            var yPos = gestureEventArgs.Y;
            var pos = new Point(xPos, yPos);
            //string sposition = "[" + xPos + "," + yPos + "]:Type" + gestureEventArgs.GestureType.ToString();
            //_overlayHelper.Show(sposition,1000);
           

            switch (gestureEventArgs.GestureType)
            {
                case RazerAPI.GestureType.Move:
                    _outputDevice.SendControlChange(Channel.Channel1, Control.SustainPedal, 0);
                    _outputDevice.SendPitchBend(Channel.Channel1, 8192);
                    foreach (KeyValuePair<Rectangle, Pitch> pair in _notes)
                    {
                        if (pointIsInsideRectangle(pos, pair.Key))
                        {
                            if (_currentPitch == pair.Value)
                                break;
                            _currentPitch = pair.Value;
                            _outputDevice.SendNoteOn(Channel.Channel1, pair.Value, 127);
                            break;
                        }
                    }
                    break;
                case RazerAPI.GestureType.Tap:
                    _outputDevice.SendControlChange(Channel.Channel1, Control.SustainPedal, 0);
                    _outputDevice.SendPitchBend(Channel.Channel1, 8192);
                    foreach (KeyValuePair<Rectangle, Pitch> pair in _notes)
                    {
                        if (pointIsInsideRectangle(pos,pair.Key))
                        {
                            _outputDevice.SendNoteOn(Channel.Channel1, pair.Value, 80);
                            break;
                        }
                    }
                    break;
            }

           
        }

        private bool pointIsInsideRectangle(Point p, Rectangle r)
        {
            var posRect = r.TransformToAncestor(this).Transform(new Point(0, 0));
            return p.X >= posRect.X && p.X <= posRect.X + r.ActualWidth && p.Y >= posRect.Y && p.Y <= posRect.Y + r.ActualHeight;
        }
       
    }
}
