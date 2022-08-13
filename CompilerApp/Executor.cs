using System.Globalization;

namespace CompilerApp
{
    public class Executor
    {
        private List<string> C { get; }
        private List<float> D { get; set; }
        private int I { get; set; }
        private int S { get; set; }

        public Executor(string path)
        {
            C = File.ReadLines(path).ToList();
            D = new List<float>();
            I = 0;
        }

        public void Execute()
        {
            while (C.Count > I)
            {
                var terms = C[I].Split(' ');
                var function = terms[0];

                var method = GetType().GetMethod(function);
                if (terms.Length == 2)
                {
                    var parameter = terms[1];
                    method?.Invoke(this, new object[]{parameter});
                }
                else
                {
                    method?.Invoke(this, Array.Empty<object>());
                }

                I++;
            }
        }
        
        // Inicia o programa
        public void INPP()
        {
            S = -1;
        }
        
        // Termina a execução do programa
        public void PARA()
        {
        }
        
        // Carrega constante k no topo da pilha D
        public void CRCT(string k)
        {
            D = D.Append(float.Parse(k, CultureInfo.InvariantCulture)).ToList();
            S++;
        }
        
        // Carrega o valor de endereço n no topo da pilha D
        public void CRVL(string n)
        {
            var element = D[int.Parse(n)];
            D = D.Append(element).ToList();
            S++;
        }

        // Soma o elemento antecessor com o topo da pilha
        public void SOMA()
        {
            D[S - 1] += D[S];
            D.RemoveAt(S--);
        }

        // Subtrai o antecessor pelo elemento do topo
        public void SUBT()
        {
            D[S - 1] -= D[S];
            D.RemoveAt(S--);
        }

        // Multiplica o elemento antecessor pelo elemento do topo
        public void MULT()
        {
            D[S - 1] *= D[S];
            D.RemoveAt(S--);
        }

        // Divide o elemento antecessor pelo elemento do topo
        public void DIVI()
        {
            D[S - 1] /= D[S];
            D.RemoveAt(S--);
        }
        
        // Inverte o sinal do topo
        public void INVE()
        {
            D[S] *= -1;
        }

        // Armazena o topo da pilha no endereço n de D
        public void ARMZ(string n)
        {
            D[int.Parse(n)] = D[S];
            D.RemoveAt(S--);
        }

        // Lê um dado de entrada para o topo da pilha
        public void LEIT()
        {
            S++;
            D = D.Append(float.Parse(Console.ReadLine(), CultureInfo.InvariantCulture)).ToList();
        }
        
        // Imprime o valor do topo da pilha na saída
        public void IMPR()
        {
            Console.WriteLine(D[S].ToString(CultureInfo.InvariantCulture));
            D.RemoveAt(S--);
        }
        
        // Reserva m posições na pilha D; m depende do tipo da variável
        public void ALME(string m)
        {
            D = D.Append(0).ToList();
            S += int.Parse(m);
        }
    }
}