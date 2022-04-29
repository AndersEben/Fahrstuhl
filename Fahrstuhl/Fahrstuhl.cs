using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fahrstuhl
{
    public class Fahrstuhl
    {
        public delegate void FahrstuhlAngekommenListener(int etage);
        public delegate void FahrstuhlPauseListener(int etage);

        private FahrstuhlAngekommenListener angekommenListener;
        private FahrstuhlPauseListener pauseListener;


        private Thread fahrstuhl;

        private int etage = 1;
        private List<int> anforderungen = new List<int>();

        private bool isRunning = true;
        private int sleep;

        private FahrstuhlState state = FahrstuhlState.Pause;

        public Fahrstuhl(int sleep = 1000, FahrstuhlAngekommenListener angekommenListener = null, FahrstuhlPauseListener pauseListener = null)
        {
            this.sleep = sleep;

            this.angekommenListener = angekommenListener;
            this.pauseListener = pauseListener;

            Start();
        }

        private void Run()
        {
            while (isRunning)
            {
                if (anforderungen.Count > 0)
                {
                    int request = GetAnfrage();

                    if (request != 0 && request != etage)
                    {
                        ChangeState(request);

                        while (anforderungen.Contains(request) && request != etage)
                        {
                            Thread.Sleep(sleep);

                            ChangeFloor();
                            angekommenListener?.Invoke(etage);

                            if (anforderungen.Contains(etage))
                            {
                                anforderungen.Remove(etage);
                                pauseListener?.Invoke(etage);

                                Thread.Sleep(sleep);
                            }
                        }

                        if (anforderungen.Count == 0)
                        {
                            state = FahrstuhlState.Pause;
                        }
                    }
                }
            }
        }

        private int GetAnfrage()
        {
            switch (state)
            {
                case FahrstuhlState.Hoch:
                    return anforderungen.Max();

                case FahrstuhlState.Runter:
                    return anforderungen.Min();

                case FahrstuhlState.Pause:
                    return anforderungen[0];

                default:
                    return 0;
            }
        }

        private void ChangeState(int anforderung)
        {
            if (anforderung > etage)
            {
                state = FahrstuhlState.Hoch;
            }
            else if (anforderung < etage)
            {
                state = FahrstuhlState.Runter;
            }
        }

        private void ChangeFloor()
        {
            switch (state)
            {
                case FahrstuhlState.Hoch:
                    etage++;
                    break;

                case FahrstuhlState.Runter:
                    etage--;
                    break;
            }
        }

        public void Start()
        {
            isRunning = true;

            fahrstuhl = new Thread(new ThreadStart(Run)) { IsBackground = true };
            fahrstuhl.Start();
        }

        public void Stop()
        {
            isRunning = false;
            fahrstuhl.Join();
        }

        public void Anforderung(int anforderung)
        {
            if (anforderung != 0 && !anforderungen.Contains(anforderung))
            {
                anforderungen.Add(anforderung);
            }
        }

        public void Abbruch(int anforderung)
        {
            anforderungen.Remove(anforderung);
        }

    }
}
