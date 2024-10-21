namespace LAB_04_EDII;

 public class DES
 {
     // Table to drop parity bits from the key
     private static readonly int[] ParityDropTable =
     [
         57, 49, 41, 33, 25, 17, 9,
         1, 58, 50, 42, 34, 26, 18,
         10, 2, 59, 51, 43, 35, 27,
         19, 11, 3, 60, 52, 44, 36,

         63, 55, 47, 39, 31, 23, 15,
         7, 62, 54, 46, 38, 30, 22,
         14, 6, 61, 53, 45, 37, 29,
         21, 13, 5, 28, 20, 12, 4
     ];

     // Table to define left shifts for key shifts
     private static readonly int[] ShiftTable =
     [
         1, 1, 2, 2, 2, 2, 2, 2,
         1, 2, 2, 2, 2, 2, 2, 1
     ];

     // Table to compress the generated key
     private static readonly int[] KeyCompressionTable =
     [
         14, 17, 11, 24, 1, 5,
         3, 28, 15, 6, 21, 10,
         23, 19, 12, 4, 26, 8,
         16, 7, 27, 20, 13, 2,
         41, 52, 31, 37, 47, 55,
         30, 40, 51, 45, 33, 48,
         44, 49, 39, 56, 34, 53,
         46, 42, 50, 36, 29, 32
     ];

     // Initial permutation table
     private static readonly int[] InitialPermutationTable = [
         58, 50, 42, 34, 26, 18, 10, 2,
         60, 52, 44, 36, 28, 20, 12, 4,
         62, 54, 46, 38, 30, 22, 14, 6,
         64, 56, 48, 40, 32, 24, 16, 8,
         57, 49, 41, 33, 25, 17, 9, 1,
         59, 51, 43, 35, 27, 19, 11, 3,
         61, 53, 45, 37, 29, 21, 13, 5,
         63, 55, 47, 39, 31, 23, 15, 7
     ];

     // Expansion permutation table used in each round
     private static readonly int[] ExpansionPermutationTable = [
          32, 1, 2, 3, 4, 5,
          4, 5, 6, 7, 8, 9,
          8, 9, 10, 11, 12, 13,
          12, 13, 14, 15, 16, 17,
          16, 17, 18, 19, 20, 21,
          20, 21, 22, 23, 24, 25,
          24, 25, 26, 27, 28, 29,
          28, 29, 30, 31, 32, 1
     ];

     // S-Box table for substitution
     private static readonly int[][,] SBoxTable =
      [
         new int[,] {
             { 14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7 },
             { 0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8 },
             { 4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0 },
             { 15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13 }
         },
         new int[,] {
             { 15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10 },
             { 3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5 },
             { 0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15 },
             { 13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9 }
         },
         new int[,] {
             { 10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8 },
             { 13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1 },
             { 13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7 },
             { 1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 12 }
         },
         new int[,] {
             { 7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15 },
             { 13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9 },
             { 10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4 },
             { 3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 5, 11, 12, 7, 2, 14 }
         },
         new int[,] {
             { 2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9 },
             { 14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6 },
             { 4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14 },
             { 11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3 }
         },
         new int[,] {
             { 12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11 },
             { 10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8 },
             { 9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6 },
             { 4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13 }
         },
         new int[,] {
             { 4, 11, 2, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1 },
             { 13, 0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8, 6 },
             { 1, 4, 11, 13, 12, 3, 7, 14, 10, 15, 6, 8, 0, 5, 9, 2 },
             { 6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0, 15, 14, 2, 3, 12 }
         },
         new int[,] {
             { 13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7 },
             { 1, 15, 13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9, 2 },
             { 7, 11, 4, 1, 9, 12, 14, 2, 0, 6, 10, 13, 15, 3, 5, 8 },
             { 2, 1, 14, 7, 4, 10, 8, 13, 15, 12, 9, 0, 3, 5, 6, 11 }
         }
      ];

     // P-Box permutation table for the final permutation
     private static readonly int[] PBoxPermutationTable =
     [
         16, 7, 20, 21, 29, 12, 28, 17,
         1, 15, 23, 26, 5, 18, 31, 10,
         2, 8, 24, 14, 32, 27, 3, 9,
         19, 13, 30, 6, 22, 11, 4, 25
     ];

     // Final permutation table to rearrange bits
     private static readonly int[] FinalPermutationTable =
     [
         40, 8, 48, 16, 56, 24, 64, 32,
         39, 7, 47, 15, 55, 23, 63, 31,
         38, 6, 46, 14, 54, 22, 62, 30,
         37, 5, 45, 13, 53, 21, 61, 29,
         36, 4, 44, 12, 52, 20, 60, 28,
         35, 3, 43, 11, 51, 19, 59, 27,
         34, 2, 42, 10, 50, 18, 58, 26,
         33, 1, 41, 9, 49, 17, 57, 25
     ];
     
     private static byte[] ProcesoDES(byte[] datosEntrada, byte[] claveCifrado, bool esAscendente)
    {
        //Arreglo de bytes resultante para los datos procesados
        byte[] datosProcesados = new byte[datosEntrada.Length];
        int cantidadBloques = datosEntrada.Length / 8; //Cada bloque es de 8 bytes (64 bits)
        byte[][] clavesRonda = GenerarClaves(claveCifrado, esAscendente); //Generar claves de ronda
        byte[] bufferBloque = new byte[8]; //Buffer para mantener el bloque actual
        byte[] mitadIzquierda = new byte[4]; //Mitad izquierda del bloque
        byte[] mitadDerecha = new byte[4]; //Mitad derecha del bloque
        byte[] mitadDerechaExpandida; //Mitad derecha expandida después de la permutación de expansión
        byte[] mitadDerechaSustituida = new byte[4]; //Resultado de la sustitución
        byte[] tempMitadDerecha; //Almacenamiento temporal para el intercambio

        //Iterar a través de cada bloque de los datos
        for (int numeroBloque = 0; numeroBloque < cantidadBloques; numeroBloque++)
        {
            // Copiar bloque actual y aplicar permutación inicial
            Array.Copy(datosEntrada, numeroBloque * 8, bufferBloque, 0, 8);
            bufferBloque = Permutar(bufferBloque, InitialPermutationTable);

            // 16 rondas de procesamiento
            for (int ronda = 0; ronda < 16; ronda++)
            {
                // Dividir el bloque en mitades izquierda y derecha
                Buffer.BlockCopy(bufferBloque, 0, mitadIzquierda, 0, 4);
                Buffer.BlockCopy(bufferBloque, 4, mitadDerecha, 0, 4);

                // Expandir la mitad derecha para el procesamiento con S-Box
                mitadDerechaExpandida = Permutar(mitadDerecha, ExpansionPermutationTable);

                // XOR con la clave de la ronda
                mitadDerechaExpandida = XOR(mitadDerechaExpandida, clavesRonda[ronda]);

                // Sustitución en las S-Box para cada sección
                for (int seccion = 0; seccion < 8; seccion++)
                {
                    // Calcular fila y columna para la búsqueda en la S-Box
                    int fila = (ObtenerBitEn(mitadDerechaExpandida, seccion * 6) << 1) | ObtenerBitEn(mitadDerechaExpandida, seccion * 6 + 5);
                    int columna = 0;
                    for (int indiceBit = 0; indiceBit < 4; indiceBit++)
                    {
                        columna |= ObtenerBitEn(mitadDerechaExpandida, seccion * 6 + indiceBit + 1) << (3 - indiceBit);
                    }

                    // Recuperar valor de la S-Box
                    int valorSBox = SBoxTable[seccion][fila, columna];
                    for (int indiceBit = 0; indiceBit < 4; indiceBit++)
                    {
                        EstablecerBitEn(mitadDerechaSustituida, seccion * 4 + indiceBit, (valorSBox >> (3 - indiceBit)) & 1);
                    }
                }

                // Permutar el resultado sustituido
                mitadDerechaSustituida = Permutar(mitadDerechaSustituida, PBoxPermutationTable);

                // XOR mitad izquierda con el resultado sustituido
                tempMitadDerecha = XOR(mitadIzquierda, mitadDerechaSustituida);

                // Intercambiar mitades para la siguiente ronda, excepto en la última ronda
                if (ronda != 15)
                {
                    Buffer.BlockCopy(mitadDerecha, 0, bufferBloque, 0, 4);
                    Buffer.BlockCopy(tempMitadDerecha, 0, bufferBloque, 4, 4);
                }
                else
                {
                    Buffer.BlockCopy(tempMitadDerecha, 0, bufferBloque, 0, 4);
                    Buffer.BlockCopy(mitadDerecha, 0, bufferBloque, 4, 4);
                }
            }

            // Aplicar la permutación final después de todas las rondas
            bufferBloque = Permutar(bufferBloque, FinalPermutationTable);
            Buffer.BlockCopy(bufferBloque, 0, datosProcesados, numeroBloque * 8, 8);
        }

        return datosProcesados;
    }
     
     public static byte[] Cifrar(byte[] datos, byte[] clave, bool agregarRelleno = true)
     {
         if (clave.Length != 8)
         {
             throw new ArgumentException("La longitud de la clave debe ser de 8 bytes");
         }

         if (agregarRelleno)
         {
             datos = AgregarRellenoPkcs7(datos, 8); // Agregar relleno si es necesario
         }

         return ProcesoDES(datos, clave, true);  // Procesar datos para cifrado
     }

     public static byte[] Descifrar(byte[] datos, byte[] clave, bool removerRelleno = true)
     {
         if (clave.Length != 8)
         {
             throw new ArgumentException("La longitud de la clave debe ser de 8 bytes");
         }

         // Asegurarse de que la longitud de los datos sea un múltiplo de 8 bytes
         if (datos.Length % 8 != 0)
         {
             throw new ArgumentException("La longitud de los datos debe ser múltiplo de 8 bytes");
         }

         // Procesar datos para descifrado
         var resultado = ProcesoDES(datos, clave, false);
         if (removerRelleno)
         {
             resultado = RemoverRellenoPkcs7(resultado); // Remover el relleno si es necesario
         }

         return resultado; // Devolver los datos descifrados
     }


     private static byte[][] GenerarClaves(byte[] claveInicial, bool esAscendente = true)
     {
         byte[][] clavesRonda = new byte[16][]; // Arreglo para almacenar las claves de ronda
         byte[] clavePermutada = Permutar(claveInicial, ParityDropTable); // Permutar clave inicial para eliminar bits de paridad
         for (int ronda = 0; ronda < 16; ronda++)
         {
             // Dividir la clave en mitades izquierda y derecha
             byte[] mitadIzquierda = SeleccionarBits(clavePermutada, 0, 28);
             byte[] mitadDerecha = SeleccionarBits(clavePermutada, 28, 28);

             // Realizar desplazamientos a la izquierda
             mitadIzquierda = DesplazarIzquierda(mitadIzquierda, 28, ShiftTable[ronda]);
             mitadDerecha = DesplazarIzquierda(mitadDerecha, 28, ShiftTable[ronda]);

             // Combinar mitades y comprimir para generar la clave de ronda
             byte[] claveCombinada = UnirClave(mitadIzquierda, mitadDerecha);
             clavesRonda[ronda] = Permutar(claveCombinada, KeyCompressionTable);
             clavePermutada = claveCombinada; // Actualizar clave permutada para la siguiente ronda
         }

         if (!esAscendente)
         {
             Array.Reverse(clavesRonda); // Invertir las claves si no es en orden ascendente (para descifrado)
         }

         return clavesRonda;
     }

     private static byte[] Permutar(byte[] origen, int[] tabla)
     {
         int longitud = (tabla.Length - 1) / 8 + 1; // Calcular longitud del resultado
         byte[] resultado = new byte[longitud];
         for (int i = 0; i < tabla.Length; i++)
         {
             EstablecerBitEn(resultado, i, ObtenerBitEn(origen, tabla[i] - 1)); // Establecer bits según la tabla de permutación
         }
         return resultado;
     }

     
     private static byte[] DesplazarIzquierda(byte[] datos, int longitud, int desplazamiento)
     {
         byte[] resultado = new byte[(longitud - 1) / 8 + 1]; // Crear nuevo arreglo de bytes para los bits desplazados
         for (int i = 0; i < longitud; i++)
         {
             int valor = ObtenerBitEn(datos, (i + desplazamiento) % longitud); // Calcular el valor desplazado
             EstablecerBitEn(resultado, i, valor);
         }

         return resultado; // Devolver el resultado desplazado
     }


     private static byte[] XOR(byte[] first, byte[] second)
     {
         byte[] result = new byte[first.Length]; // Resultant byte array
         for (int i = 0; i < first.Length; i++)
         {
             result[i] = (byte)(first[i] ^ second[i]); // Perform XOR operation
         }

         return result;  // Return XOR result
     }

     private static int ObtenerBitEn(byte[] datos, int posicion)
     {
         int posByte = posicion / 8; // Calcular la posición del byte
         int posBit = posicion % 8; // Calcular la posición del bit
         return datos[posByte] >> (7 - posBit) & 1; // Devolver el valor del bit
     }

     private static void EstablecerBitEn(byte[] datos, int posicion, int valor)
     {
         int posByte = posicion / 8; // Calcular la posición del byte
         int posBit = posicion % 8; // Calcular la posición del bit            
         if (valor == 1)
             datos[posByte] |= (byte)(1 << (7 - posBit)); // Establecer el bit a 1
         else
             datos[posByte] &= (byte)~(1 << (7 - posBit)); // Establecer el bit a 0
     }

     private static byte[] SeleccionarBits(byte[] origen, int inicio, int cantidad)
     {
         byte[] resultado = new byte[(cantidad - 1) / 8 + 1]; // Crear arreglo para el resultado
         for (int i = 0; i < cantidad; i++)
         {
             EstablecerBitEn(resultado, i, ObtenerBitEn(origen, inicio + i)); // Establecer bits en el arreglo resultado
         }

         return resultado; // Devolver los bits seleccionados
     }



     private static byte[] UnirClave(byte[] mitadIzquierda, byte[] mitadDerecha)
     {
         byte[] resultado = new byte[7]; // Asignar nuevo arreglo de bytes para el resultado
         for (int i = 0; i < 3; i++)
         {
             resultado[i] = mitadIzquierda[i];
         }
         for (int i = 0; i < 4; i++)
         {
             int val = ObtenerBitEn(mitadIzquierda, 24 + i); //24-27
             EstablecerBitEn(resultado, 24 + i, val);
         }
         for (int i = 0; i < 28; i++)
         {
             int val = ObtenerBitEn(mitadDerecha, i);
             EstablecerBitEn(resultado, 28 + i, val);
         }
         return resultado;
     }

     public static byte[] AgregarRellenoPkcs7(byte[] datos, int tamanoBloque)
     {
         if (datos == null)
         {
             throw new ArgumentNullException(nameof(datos), "Los datos no pueden ser nulos");
         }

         if (tamanoBloque <= 0)
         {
             throw new ArgumentException("El tamaño del bloque debe ser mayor que cero", nameof(tamanoBloque));
         }

         int conteo = datos.Length;
         int restoRelleno = conteo % tamanoBloque;
         int tamanoRelleno = tamanoBloque - restoRelleno;

         if (tamanoRelleno == 0)
         {
             tamanoRelleno = tamanoBloque;
         }

         byte[] datosConRelleno = new byte[datos.Length + tamanoRelleno];
         Buffer.BlockCopy(datos, 0, datosConRelleno, 0, datos.Length);

         byte byteRelleno = (byte)tamanoRelleno;
         for (int i = datos.Length; i < datosConRelleno.Length; i++)
         {
             datosConRelleno[i] = byteRelleno;
         }

         return datosConRelleno;
     }

     
     public static byte[] RemoverRellenoPkcs7(byte[] arregloBytesRellenados)
     {
         if (arregloBytesRellenados == null)
         {
             throw new ArgumentNullException(nameof(arregloBytesRellenados), "El arreglo de bytes rellenados no puede ser nulo");
         }

         if (arregloBytesRellenados.Length == 0)
         {
             throw new ArgumentException("El arreglo de bytes rellenados no puede estar vacío", nameof(arregloBytesRellenados));
         }

         int tamanoRelleno = arregloBytesRellenados[arregloBytesRellenados.Length - 1];
         if (tamanoRelleno < 1 || tamanoRelleno > arregloBytesRellenados.Length)
         {
             throw new ArgumentException("Tamaño de relleno inválido.");
         }

         for (int i = arregloBytesRellenados.Length - tamanoRelleno; i < arregloBytesRellenados.Length; i++)
         {
             if (arregloBytesRellenados[i] != tamanoRelleno)
             {
                 throw new ArgumentException("Relleno inválido.");
             }
         }

         int longitudResultado = arregloBytesRellenados.Length - tamanoRelleno;
         byte[] resultado = new byte[longitudResultado];
         Buffer.BlockCopy(arregloBytesRellenados, 0, resultado, 0, longitudResultado);

         return resultado;
     }

 }