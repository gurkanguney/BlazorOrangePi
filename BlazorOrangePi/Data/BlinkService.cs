using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Device.Gpio;
using Iot.Device.DHTxx;
using Iot.Device.OneWire;
using Iot.Units;
using System.Threading;
using Iot.Device.Ssd13xx;
using System.Device.I2c;
using Iot.Device.Ssd13xx.Commands;

namespace BlazorOrangePi.Data
{
    public class BlinkService
    {
        // Events
        public delegate void PinDurumuDegistiDelegete(object sender, BlinkServiceChangedEventArgs args);
        public delegate void SicaklikDegistiDelegate(object sender, SicaklikDegistiEventArgs args);

        public event PinDurumuDegistiDelegete OnPinDurumuDegisti;
        public event SicaklikDegistiDelegate OnSicaklikDegisti;



        public static GpioController gpioController;
        public static int ledPin = 110;  //PD14
        private Timer zamanlayici;
        public Exception SonHata;
        private double _sicaklik;
        private bool _ledDurum;


        public double Sicaklik
        {
            get { return _sicaklik; }
            private set
            {
                if (_sicaklik != value)
                {
                    OnSicaklikDegisti(this, new SicaklikDegistiEventArgs(value, _sicaklik));
                }
                _sicaklik = value;
            }
        }

        public bool LedDurum
        {
            get { return _ledDurum; }
            private set
            {
                if (_ledDurum != value)
                {
                    OnPinDurumuDegisti(this, new BlinkServiceChangedEventArgs(value, _ledDurum));
                }
                _ledDurum = value;
            }

        }

        public void ServiceInit()
        {
            zamanlayici = new Timer(new TimerCallback(async _ =>
            {
                await DS18B20SicaklikOku();
            }), null, 1000, 2000);
        }
        public BlinkService()
        {
            try
            {
                gpioController = new GpioController();

                if (!gpioController.IsPinOpen(ledPin))
                {
                    gpioController.OpenPin(ledPin, PinMode.Output);
                }


            }
            catch (Exception hata)
            {
                SonHata = hata;
            }
        }

        public void LedIslem()
        {
            if (!LedDurum)
            {
                gpioController.Write(ledPin, PinValue.High);
                LedDurum = true;
            }
            else
            {
                gpioController.Write(ledPin, PinValue.Low);
                LedDurum = false;
            }
        }
        public async Task DS18B20SicaklikOku()
        {
            try
            {
                string busId = OneWireBus.EnumerateBusIds().First();
                string deviceId = OneWireDevice.EnumerateDeviceIds().First().devId;
                OneWireThermometerDevice ds18B20Dev = new OneWireThermometerDevice(busId, deviceId);
                Temperature sicaklik = await ds18B20Dev.ReadTemperatureAsync();

                Sicaklik = sicaklik.Celsius;
            }
            catch (Exception hata)
            {
                SonHata = hata;
            }

        }
    }




    public class BlinkServiceChangedEventArgs : EventArgs
    {
        public bool OncekiDurum { get; set; }
        public bool Durum { get; set; }

        public BlinkServiceChangedEventArgs(bool yeniDurum, bool oncekiDurum)
        {
            this.Durum = yeniDurum;
            this.OncekiDurum = oncekiDurum;
        }
    }

    public class SicaklikDegistiEventArgs : EventArgs
    {
        public double OncekiSicaklik { get; set; }
        public double SonSicaklik { get; set; }

        public SicaklikDegistiEventArgs(double yenisi, double eskisi)
        {
            this.OncekiSicaklik = eskisi;
            this.SonSicaklik = yenisi;
        }
    }

}
