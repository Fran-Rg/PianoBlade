// ---------------------------------------------------------------------------------------
//  <copyright file="Provider.cs" company="SharpBlade">
//      Copyright © 2013-2014 by Adam Hellberg and Brandon Scott.
//
//      Permission is hereby granted, free of charge, to any person obtaining a copy of
//      this software and associated documentation files (the "Software"), to deal in
//      the Software without restriction, including without limitation the rights to
//      use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
//      of the Software, and to permit persons to whom the Software is furnished to do
//      so, subject to the following conditions:
//
//      The above copyright notice and this permission notice shall be included in all
//      copies or substantial portions of the Software.
//
//      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//      IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//      FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//      AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
//      WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//      CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//      Disclaimer: SharpBlade is in no way affiliated
//      with Razer and/or any of its employees and/or licensors.
//      Adam Hellberg does not take responsibility for any harm caused, direct
//      or indirect, to any Razer peripherals via the use of SharpBlade.
//
//      "Razer" is a trademark of Razer USA Ltd.
//  </copyright>
// ---------------------------------------------------------------------------------------

namespace Sharparam.Lib
{
    using System.Windows;

    using Sharparam.SharpBlade.Native;
    using Sharparam.SharpBlade.Razer;
    using Sharparam.SharpBlade.Razer.Events;

    public static class Provider
    {
        private static RazerManager _razer;

        public static RazerManager Razer
        {
            get
            {
                if (_razer == null)
                {
                   // _razer = RazerManager.Instance;
                    //Create SharpBlade RazerManager object
                    _razer = new RazerManager();
                   // _razer.BlankTouchpadImagePath = @"Default\Images\tp_blank.png";
                   // _razer.DisabledDynamicKeyImagePath = @"Default\Images\dk_disabled.png";

                    //Add the event for checking the App Status
                    _razer.AppEvent += OnAppEvent;
                }

                return _razer;
            }
        }

        /// <summary>
        /// This event handler is fired when an app event happens
        /// </summary>
        /// <param name="sender">The object that called this event</param>
        /// <param name="appEventArgs">The arguments that give details as to the app event</param>
        private static void OnAppEvent(object sender, AppEventEventArgs e)
        {
            /* Just an example here.  If the app becomes deactivated, is closed or force quit, then close the app.
             * Potentially you could run a App Lifecycle here where when it's deactivated, it unbinds any data connection
             * And when the app is activated again, it reloads the data source and opens back up
             */
            if (e.Type == RazerAPI.AppEventType.Deactivated || e.Type == RazerAPI.AppEventType.Close ||
                e.Type == RazerAPI.AppEventType.Exit)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
