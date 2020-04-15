# covid19mxinfodownload
Azure function que descarga cada día los documentos informativos del COVID-19 de México

La instancia en ejecución de la Fnction por el momento está dejando los datos en:

https://commons.blob.core.windows.net/covid19/history/MM/DD/FFFFF

Donde MM es el mes y DD es el día en el que se almacenan los archivos, y FFFFF es el nombre del archivo.

Por ejemplo los siguientes URIs:

https://commons.blob.core.windows.net/covid19/history/04/08/Comunicado_Tecnico_Diario.pdf
https://commons.blob.core.windows.net/covid19/history/04/08/Tabla_casos_positivos.pdf
https://commons.blob.core.windows.net/covid19/history/04/08/Tabla_casos_sospechosos.pdf

Corresponden a los descargados el 8 de abril.


