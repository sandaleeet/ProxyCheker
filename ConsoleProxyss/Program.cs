using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Numerics;
using System.Threading;

namespace ProxyChekers
{ //https://free-proxy-list.net/
    internal class Program
    {
        //статистическое поле сохраняющее дальше в коде последнее значение.
        public static int mincoun = 0;

        // Метод для чека прокси. min & max диапозон для проксей
        public static void Cheker(string[] ms, int min, int max)
        {
            //оборочивание в цикл for для отлова исключение и перехождения к next proxy
            for (var j = min; j <= max; j++)
            {
                //переменная для блока catch. потому что i цикла for из блока try недоступна
                var cv = 0;
                try
                {
                    for (var i = min; i <= max; i++)
                    {
                        //чек прокси
                        cv = i;
                        var proxy = new WebProxy(ms[i], true);
                        var req = WebRequest.CreateHttp("https://www.google.com/");
                        req.Proxy = proxy;
                        req.Timeout = 2000;
                        var Response = (HttpWebResponse)req.GetResponse();
                        var RespString = new StreamReader(Response.GetResponseStream()).ReadToEnd();
                        Console.WriteLine(ms[cv] + "живой");
                    }

                }
                //исключения
                catch (WebException)
                {
                    Console.WriteLine(ms[cv] + "мертвец");

                }
            }
        }

        private static void Main()
        {
            var list = new List<string>();

            int min;
            int max;
            // кол-во прокси на один поток
            int proxyon1thread;
            // кол-во потоков
            var nathreads = 10;
            //для foreach
            var chekadd = false;
            string writetoprox = null;
            //берем прокся
            var file = File.ReadAllText(@"E:\projects\proxy.txt");

            //считывание прокси без пробелов и тд в листе
            foreach (var r in file)
                if ((r != ' ') & (r != '\r') & (r != '\n'))
                {
                    writetoprox += r;
                    chekadd = false;
                }
                else
                {
                    if (chekadd == false) list.Add(writetoprox);
                    chekadd = true;
                    writetoprox = null;
                }
            // Делаю переменную ссылки на массив типа string, а затем присваиваю ей коллекцию, которую с помощью метода ToArray я перевёл в массив.
            var ms = list.ToArray();
            proxyon1thread = ms.Length / nathreads;
            // массив  потоков
            var th = new Thread[10];
            // присвоение значения пременным
            min = 0;
            max = 0;
            int mt;
            //в этом цикле for я добавляю в массив потоков лямбда-выражение, которое вмещает в себя метод Checker
            for (var i = 1; i <= nathreads; i++)
            {
                //конечное значение для проверки
                mt = proxyon1thread * i;
                //диапозон проверки прокси от 0 и до proxyon1thread
                if (i == 0)
                {
                    th[i - 1] = new Thread(() => Cheker(ms, 0, proxyon1thread));
                    //запоминаем индекс проверенного прокси
                    mincoun = proxyon1thread;
                }
                else
                {
                    //добавляем поток, где от значений переменной mincoun до значения переменной mt ( mt отображет тот диапазон, до которого метод должен проверить ) метод проверяет прокси.
                    th[i - 1] = new Thread(() => Cheker(ms, mincoun, mt));
                    // запоминаем значение которое было конечным. до которого мы проверяли
                    mincoun = mt;
                }
            }
            //запуск потоков в цикле
            for (var i = 0; i < th.Length; i++) th[i].Start();
        }
        
    }
}
            
        
        

    
