Требования к заданию:
Разработать консольное приложение на C# для поблочного сжатия и распаковки файлов с помощью System.IO.Compression.GzipStream.
Для сжатия исходный файл делится на блоки одинакового размера, например, в 1 мегабайт. 
Каждый блок сжимается и записывается в выходной файл независимо от остальных блоков.
При работе с потоками допускается использовать только базовые классы и объекты синхронизации (Thread, Manual/AutoResetEvent, Monitor, Semaphor, Mutex) и не допускается использовать async/await, ThreadPool, BackgroundWorker, TPL.
Параметры программы, имена исходного и результирующего файлов должны задаваться в командной строке следующим образом:
GZipTest.exe compress/decompress [имя исходного файла] [имя результирующего файла]
В случае успеха программа должна возвращать 0, при ошибке возвращать 1.
__________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________
Для реализации данной задачи использовались паттерны Producer-Consumer, Strategy.
Программа принимает массив из трех строк compress/decompress [имя исходного файла] [имя результирующего файла], 
производит грубый анализ строк на предмет соответствия требованиям входных параметров,
в зависимости от 1 строки вызывает compress или decompress и передает туда остальные два параметра.
Compress или decompress выполняется в многопоточной среде, количество потоков в которой зависит от количества ядер.
Чтение файла происходят поблоково в отдельном потоке, файл разбивается на блоки и ложится в буффер_на_чтение, далее отправляется на
компрессию или декомпрессию, далее ложится в другой буффер_на_сжатие и отправляется на запись в конечный файл.
Для удобства был сделан прогресс бар.
