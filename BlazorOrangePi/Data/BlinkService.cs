using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Device.Gpio;


namespace BlazorOrangePi.Data
{
    public class BlinkService
    {
        public static GpioController gpioController = new GpioController();
        public static int ledPin = 68;
        public bool ledDurum = false;
        public BlinkService()
        {
            if (!gpioController.IsPinOpen(ledPin))
            {
                gpioController.OpenPin(ledPin, PinMode.Output);
            }
            else
            {
                ledDurum = (PinValue.High == gpioController.Read(ledPin));
            }
        }
        public void LedIslem(bool toggle)
        {
            if (toggle)
            {
                gpioController.Write(ledPin, PinValue.High);
                ledDurum = true;
            }
            else
            {
                gpioController.Write(ledPin, PinValue.Low);
                ledDurum = false;
            }

        }
    }
}
