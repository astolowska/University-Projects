//Rextester.Program.Main is the entry point for your code. Don't change it.
//Compiler version 4.0.30319.17929 for Microsoft (R) .NET Framework 4.5

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions; 

/*
Przykladowe labirynty:

1)
.S...
.XXX.
...MX

2) - nie rozwiazalny
.S.XX
.X.X.
...XM

3) - z petla
.S.XX
.X...
...XM

 */

namespace Rextester
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var wczytaj = new Wczytuje();
                ILabirynt lab = wczytaj.Wczytaj();

                var przechodzenie = new Przechodze();
                przechodzenie.Przejdz(lab);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Console.WriteLine("Koniec");
            }
            
        }
    }
    
    public interface IPrzechodze
    {
        void Przejdz(ILabirynt labirynt);
    }
    
    public interface IWczytuje
    {
        ILabirynt Wczytaj();
    }
    
    public interface ILabirynt
        
    {
        IPole PodajPole(Pozycja wyj, Kierunek kierunek);
        
        IPole GdzieJestStart();
        
        IPole GdzieJestMeta();
        
        void DodajPole(IPole pole);
        
        void Pokaz();
    }
    
    public interface IPole
    {
        Pozycja Pozycja{get;}
        
        int IleRazyOdwiedzone{get;set;}
        
        bool CzySciana{get;}
        bool CzyMeta{get;}
        bool CzyStart{get;}
        bool CzyPrzejscie{get;}
                
    }
    
    public enum Kierunek
    {
        Lewo,
        Prawo,
        Gora,
        Dol
    }
    

    
    public class Wczytuje : IWczytuje
    {
        private ILabirynt Labirynt {get;set;}
        private Pozycja BiezacaPozycja {get;set;}
        
        public ILabirynt Wczytaj()
        {
            Console.WriteLine("Wczytuje");
            Labirynt = new Labirynt();
            
            BiezacaPozycja = new Pozycja{X=0,Y=0};
            
            List<String> linie = WczytajLinie();
            
            Console.WriteLine(linie.Count);
            
            foreach(string linia in linie)
            {
                AnalizujLinie(linia);
                BiezacaPozycja.Y++;
                BiezacaPozycja.X = 0;
            }
            
            Console.WriteLine("Wczytalem");
            return Labirynt;
        }
        
        private void AnalizujLinie(string linia)
        {
            foreach(var znak in linia)
            {
                AnalizujZnak(znak);
                BiezacaPozycja.X++;
            }
        }
        
        public void AnalizujZnak(char znak)
        {
            IPole pole = UtworzPole(znak);
            Labirynt.DodajPole(pole);        
        }
        
        private IPole UtworzPole(char znak) //funkcja fabrykujaca pola.
        {
            switch(znak)
            {
                case '.':
                {
                    return new Przejscie(BiezacaPozycja);
                }
                case 'X':
                {
                    return new Sciana(BiezacaPozycja);
                }
                case 'S':
                {
                    return new Start(BiezacaPozycja);
                }
                case 'M':
                {
                    return new Meta(BiezacaPozycja);
                }
                default:
                    throw new Exception(String.Format("Nie rozpoznany znak:{0} w pozycji {1}", znak, BiezacaPozycja));
            }
        }
        
        private List<string> WczytajLinie()
        {
            var wynik = new List<string>();
            var pusta = true;
            do
            {
                string linia = Console.ReadLine();
                pusta = String.IsNullOrWhiteSpace(linia);
                if(!pusta)
                {
                    wynik.Add(linia);
                }                
            }
            while(!pusta);
            
            return wynik;
        }
    }
    
    public class Przechodze : IPrzechodze
    {
        private int NumerKroku {get;set;}
        private Pozycja Aktualna {get;set;}
        private Pozycja Meta{get;set;}
        private ILabirynt Labirynt {get;set;}

        private readonly List<Kierunek> _wszystkieKierunki;
        private bool CzyKoniec {get;set;}


        public Przechodze()
        {
            NumerKroku = 0;
            _wszystkieKierunki = new List<Kierunek>{Kierunek.Lewo, Kierunek.Prawo, Kierunek.Dol, Kierunek.Gora};
        }
        
        public void Przejdz(ILabirynt labirynt)
        {
            Console.WriteLine("Przechodze");
            Labirynt = labirynt;
            
            Przygotuj();
            
            do
            {
                WykonajKrok();
                Labirynt.Pokaz();
                CzyKoniec = Aktualna.Rowne(Meta);
            }
            while(!CzyKoniec);
        }

        

        private void Przygotuj()
        {
            var start = Labirynt.GdzieJestStart();
            if(start == null)
            {
                throw new Exception("Nie ma startu");
            }
            if(start.Pozycja == null)
            {
                throw new Exception("Nie ma pozycji startu");
            }
            Aktualna = start.Pozycja;
            
            var meta = Labirynt.GdzieJestMeta();
            if(meta == null)
            {
                throw new Exception("Nie ma mety");
            }
            if(meta.Pozycja == null)
            {
                throw new Exception("Nie ma pozycji mety");
            }
            Meta = meta.Pozycja;
            Labirynt.Pokaz();
        }
        
        private void WykonajKrok()
        {
            NumerKroku++;
            Console.WriteLine("Krok nr:"+ NumerKroku);

            var mozliweRuchy = AnalizujRuch();
            var potencjalnieNajlepszy = mozliweRuchy.First();
            WykonajRuch(potencjalnieNajlepszy);
  
        }

        private IList<Ruch> AnalizujRuch()
        {
            var ruchy = new List<Ruch>();
            foreach(Kierunek kierunek in _wszystkieKierunki)
            {
                IPole docelowe = Labirynt.PodajPole(Aktualna, kierunek);
                var ruch = new Ruch(kierunek, docelowe);
                ruchy.Add(ruch);
            }
            ruchy = ruchy
                .Where(r => r.CzyMogeIsc) //wybierz tylko te do ktorych mozna isc
                .OrderBy(r => r.IleRazyOdwiedzone) //uszereguj wg ilosci odwiedzen
                .ToList(); //zamien znow na liste
            return ruchy;
        }

        private void WykonajRuch(Ruch ruch)
        {
            if(!ruch.CzyMogeIsc)
            {
                throw new Exception("Nie moge nigdzie isc - w kolo same sciany");
            }

            if(ruch.IleRazyOdwiedzone >=4)
            {
                throw new Exception("Nie Moge nigdzie isc - labirynt nie ma wyjscia");
            }

            IPole docelowe = ruch.PoleDocelowe;
            docelowe.IleRazyOdwiedzone++;
            Aktualna = docelowe.Pozycja;
        }
    }
    
    public class Ruch
    {
        public Kierunek Kierunek {get;set;}
        public IPole PoleDocelowe {get;set;}
        public bool CzyMogeIsc 
        {
            get
            {
                return (PoleDocelowe != null) && !(PoleDocelowe.CzySciana);
            }
        }

        public int IleRazyOdwiedzone
        {
            get
            {
                if(CzyMogeIsc)
                {
                    return PoleDocelowe.IleRazyOdwiedzone;
                }
                throw new Exception("Nie powinnam sprawdzac ile razy bylam w polu do ktorego nie moge isc");
            }
        }

        public Ruch(Kierunek kierunek, IPole docelowe)
        {
            Kierunek = kierunek;
            PoleDocelowe = docelowe;           
        }

    }

    public class Labirynt : ILabirynt
    {
        private List<List<IPole>> Pola{get;set;}
        
        private int IleWierszy {get;set;}
        private int IleKolumn{get;set;}
        
        public Labirynt()
        {
            Pola = new List<List<IPole>>();
        }
        
        public IPole PodajPole(Pozycja wyjsciowe, Kierunek kierunek)
        {
            IPole pole = ZnajdzPole(wyjsciowe, kierunek);
            return pole;
        }
        
        public IPole GdzieJestStart()
        {
            foreach(var wiersz in Pola)
            {
                foreach(var pole in wiersz)
                {
                    if(pole.CzyStart)
                    {
                        return pole;
                    }
                }
            }
            return null;
        }
        
        public IPole GdzieJestMeta()
        {
            foreach(var wiersz in Pola)
            {
                foreach(var pole in wiersz)
                {
                    if(pole.CzyMeta)
                    {
                        return pole;
                    }
                }
            }
            return null;
        }
        
        public void DodajPole(IPole p)
        {
            int y = p.Pozycja.Y;
            List<IPole> wiersz;
            if(Pola.Count <= y)
            {
                wiersz = new List<IPole>();
                Pola.Add(wiersz);
                IleWierszy++;
            }
            else
            {
                wiersz = Pola[y];
            }
            
            wiersz.Add(p);
            if(wiersz.Count > IleKolumn)
            {
                IleKolumn = wiersz.Count;
            }
            
        }
        
        public void Pokaz()
        {
            foreach(var wiersz in Pola)
            {
                foreach(var pole in wiersz)
                {
                    Console.Write(pole);
                }
                Console.WriteLine();
            }
        }
            
        private Pozycja ZnajdzPozycje(Pozycja p, Kierunek k) //metoda fabrykująca pozycje
        {
            switch(k)
            {
                case Kierunek.Lewo:
                {
                    return new Pozycja {X = p.X+1, Y = p.Y};
                }
                case Kierunek.Prawo:
                {
                    return new Pozycja {X = p.X-1, Y = p.Y};
                }
                case Kierunek.Gora:
                {
                    return new Pozycja {X = p.X, Y = p.Y-1};
                }
                case Kierunek.Dol:
                {
                    return new Pozycja {X = p.X, Y = p.Y+1};
                }
                default:
                    return null;
            }
        }    
        
        private IPole ZnajdzPole(Pozycja p, Kierunek k)
        {
            Pozycja docelowa = ZnajdzPozycje(p,k);
            if( (docelowa.X < 0) || (docelowa.X >= IleKolumn))
            {
                return null;
            }
            if( (docelowa.Y < 0) || (docelowa.Y >= IleWierszy))
            {
                return null;
            }
            return Pola[docelowa.Y][docelowa.X];
        }
        
    }
    
    public abstract class Pole : IPole
    {
        public Pole(Pozycja pozycja)
        {
            Pozycja = new Pozycja
            {
                X=pozycja.X,
                Y=pozycja.Y
            }; //wazne jest robienie kopii pozycji, gdyz unikamy bledu odwolania sie do niepożądanego obiektu
            CzySciana = false;
            CzyMeta = false;
            CzyStart = false;
            CzyPrzejscie = false;
            IleRazyOdwiedzone = 0;
        }
        
        public Pozycja Pozycja {get; private set;}
        public int IleRazyOdwiedzone {get;set;}
        public bool CzySciana{get; protected set;}
        public bool CzyMeta{get; protected set;}
        public bool CzyStart{get; protected set;}
        public bool CzyPrzejscie{get; protected set;}


        protected abstract string Napis {get;}

        public override string ToString()
        {
            return String.Format("({0}{1})", Napis, IleRazyOdwiedzone);
        }

    }
    
    public class Przejscie : Pole
    {
        public Przejscie(Pozycja pozycja)
            : base(pozycja)
        {
            CzyPrzejscie = true;
        }
        
        protected override string Napis
        {
            get {return " ";}
        }
    }
    
    public class Sciana : Pole
    {
        public Sciana(Pozycja pozycja)
            : base(pozycja)
        {
            CzySciana = true;
        }
        
        protected override string Napis
        {
            get {return "x";}
        }
        
    }    

    public class Start : Pole
    {
        public Start(Pozycja pozycja)
            : base(pozycja)
        {
            CzyStart = true;
        }
        
        protected override string Napis
        {
            get{return "s";}
        }
        
    }    

    public class Meta : Pole
    {
        public Meta(Pozycja pozycja)
            : base(pozycja)
        {
            CzyMeta = true;
        }
        
        protected override string Napis
        {
            get{return "m";}
        }
        
    }   
    
    public class Pozycja
    {
        public int X{get;set;}
        public int Y{get;set;}
        
        public bool Rowne(Pozycja p)
        {
            return (p.X == X) && (p.Y == Y);
        }
        
        public override string ToString()
        {
            return string.Format("[{0},{1}]",X,Y);
        }
    }
}