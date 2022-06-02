using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rextester
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int[,] plansza = new int[,] {
                { 5, 3, 0, 0, 7, 0, 0, 0, 0 },
                { 6, 0, 0, 1, 9, 5, 0, 0, 0 },
                { 0, 9, 8, 0, 0, 0, 0, 6, 0 },
                { 8, 0, 0, 0, 6, 0, 0, 0, 3 },
                { 4, 0, 0, 8, 0, 3, 0, 0, 1 },
                { 7, 0, 0, 0, 2, 0, 0, 0, 6 },
                { 0, 6, 0, 0, 0, 0, 2, 8, 0 },
                { 0, 0, 0, 4, 1, 9, 0, 0, 5 },
                { 0, 0, 0, 0, 8, 0, 0, 7, 9 }
            };

          plansza.rozwiazSudoku(plansza);
            
        }
    }
    
    public interface IPlansza
{
	void wyswietlPlansze(Plansza plansza);
}


public interface ISprawdz
{
	static czyPelna(Plansza plansza);
}

public interface IRozwiazSudoku
{
	static dostepneCyfry(Plansza plansza, i, j);
	static rozwiazSudoku(Plansza plansza);
	
}


public class Plansza : IPlansza, ISprawdz
{
	public int x = 0;
	public int y = 0;
	
	public int[,] Plansza = new int[9, 9];
	
	//Metoda wyswietlajaca siatke
	public void wyswietlPlansze(Plansza plansza){
			Console.WriteLine("++++++++++++++++++++++");
			for(x = 0; x < 9; x++){
				if(x == 3 || x == 6){
					Console.WriteLine("++++++++++++++++++++++");
				}
				for(y = 0; y < 9; y++){
					if(y == 3 || y == 6){
						Console.Write("+ ");
					}
					Console.Write(plansza[x, y] + " ");
				}
				Console.WriteLine;
			}
			Console.WriteLine("++++++++++++++++++++++");
		}
		
		//Metoda sprawdzajaca, czy plansza jest w pelni uzupelniona
		//Zwraca true, jesli zapelniona, false jesli nie
		//Jesli znajdzie conajmniej jedno 0 = false
		public static czyPelna(Plansza plansza){
			for(x = 0; x < 9; x++){
				for(y = 0; y < 9; y++){
					if(plansza[x, y] == 0){
						return false;
					}
				}
			}
			return true;
		}
}


public class RozwiazSudoku : Plansza, IRozwiazSudoku
{
	//Wynajduje wszystkie mozliwe, nieuzyte cyfry poprzez
	//sprawdzenie kolumny, rzedu oraz kwadratu 3x3
		public static dostepneCyfry(Plansza plansza, i, j){
		IDictionary<int, int> spisDostepnych = new Dictionary<int, int>();
		public int k, l = 0;
		
		for(x = 1; x < 10; x++){
			 spisDostepnych.Add(x, 0); 
		}
		
		 //sprawdzenie dla rzedu
		 for(y = 0; y < 9; y++){
			 if(plansza[i, y] != 0){
				 spisDostepnych[plansza[i, y]] = 1;
			 }
		}
		
		//sprawdzenie dla kolumny
		for(x = 0; x < 9; x++){
			 if(plansza[x, j] != 0){
				 spisDostepnych[plansza[x, j]] = 1;
			 }
		}
		
		//sprawdzenie dla kwadratu 3x3, horyzontalnie i wertykalnie
		if(i >= 0 && i <= 2){
			horyz = 0;
		} else if(i >= 3 && i <= 5){
			horyz = 3;
		} else{
			horyz = 6;
		}
		
		if(j >= 0 && j <= 2){
			wer = 0;
		} else if(j >= 3 && j <= 5){
			wer = 3;
		} else{
			wer = 6;
		}
		
		for(x = horyz; x < horyz + 3; x++){
			for(y = wer; y < wer + 3; y++){
				if(plansza[x, y] != 0){
					spisDostepnych[plansza[x, y]] = 1;
				}
			}
		}
		
	
	for(x = 1; x < 10; x++){
		if(spisDostepnych[x] == 0){
			spisDostepnych[x] = x;
		} else{
			spisDostepnych[x] = 0;
		}
	}
	return spisDostepnych;
	}
	
	
	
	//Rozwiazanie planszy z uzyciem rekurencji i wyswietlenie jej.
	public static rozwiazSudoku(Plansza plansza){
			
		public int i, j = 0;
		
		IList<int> dostepne = new List<int>();
				
	   //Jezeli plansza jest rozwiazana, wyswietl komunikat i wypisz wyniki
		if(czyPelna(plansza) == true){
			Console.WriteLine("Sudoku zostalo rozwiazane.");
			pokazPlansze(plansza);
			return;
		} else{
			//Jezeli nie, poszukuje pierwszego wolnego miejsca (0)
			for(x = 0; x < 9; x++){
				for(y = 0; y < 9; y++){
					if(plansza[x, y] == 0){
						i = x;
						j = y;
						break;
					}
				}
			}
		}

		//zapisz wszystkie mozliwe cyfry dla i, j
		//dostepne = dostepneCyfry(plansza, i, j);
		dostepne.Add(dostepneCyfry(plansza, i, j));

		//Przejscie przez wszystkie elementy dostepne i rekurencyjne wywolanie funkcji
		for(x = 1; x < 10; x++){
			if(dostepne[x] != 0){
				plansza[i, j] = dostepne[x];
				rozwiazSudoku(plansza);
			}
		}
		//COFANIE SIE - w przypadku, gdy nie ma prawidlowej opcji
		//indeks i, j jest ustawiany na 0 i program resetuje pola
		plansza[i, j] = 0;

	}
}
}