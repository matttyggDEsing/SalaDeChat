
# SalaDeChat

SalaDeChat es una aplicación de chat desarrollada en C#, compuesta por dos módulos principales: ClienteApp y ServidorApp. El objetivo de este proyecto es facilitar la comunicación en tiempo real entre múltiples usuarios a través de una interfaz de consola.

## Características

- **Servidor multicliente**: El servidor puede manejar múltiples conexiones simultáneamente.
- **Cliente interactivo**: Los clientes pueden enviar y recibir mensajes en tiempo real.
- **Interfaz de consola**: Tanto el cliente como el servidor operan a través de la línea de comandos.
- **Comunicación basada en sockets**: Utiliza sockets TCP para la transmisión de datos.

## Requisitos

- .NET SDK (versión compatible con el proyecto)
- Sistema operativo compatible con .NET (Windows, Linux, macOS)

## Instalación y ejecución

1. **Clonar el repositorio**:

   ```bash
   git clone https://github.com/matttyggDEsing/SalaDeChat.git
   cd SalaDeChat
   ```

2. **Compilar el servidor**:

   ```bash
   cd ServidorApp
   dotnet build
   ```

3. **Ejecutar el servidor**:

   ```bash
   dotnet run
   ```

4. **Compilar el cliente**:

   ```bash
   cd ../ClienteApp
   dotnet build
   ```

5. **Ejecutar el cliente**:

   ```bash
   dotnet run
   ```

*Nota*: Asegúrate de que el servidor esté en ejecución antes de iniciar el cliente.

## Uso

1. Inicia el servidor ejecutando el comando correspondiente en la carpeta `ServidorApp`.
2. En una o varias terminales diferentes, inicia el cliente desde la carpeta `ClienteApp`.
3. Cada cliente podrá enviar mensajes que serán recibidos por todos los demás clientes conectados.

## Contribuciones

Las contribuciones son bienvenidas. Si deseas mejorar el proyecto o corregir errores, por favor sigue estos pasos:

1. Haz un fork del repositorio.
2. Crea una nueva rama para tu funcionalidad o corrección:

   ```bash
   git checkout -b nombre-de-tu-rama
   ```

3. Realiza tus cambios y haz commit:

   ```bash
   git commit -m "Descripción de tus cambios"
   ```

4. Haz push a tu rama:

   ```bash
   git push origin nombre-de-tu-rama
   ```

5. Abre un Pull Request en GitHub.

## Licencia

Este proyecto está bajo la Licencia MIT. Consulta el archivo `LICENSE` para más detalles.
