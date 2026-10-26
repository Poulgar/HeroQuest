# HeroQuest

Compendio personal de héroes, monstruos, equipo, artefactos y miscelánea de HeroQuest.

## Abrir la web en los móviles durante una partida

La forma más sencilla y gratuita es iniciar la web en un portátil conectado al mismo Wi‑Fi que los móviles. El portátil hace de servidor mientras la partida está en marcha. Los demás solo necesitan el navegador del teléfono: no necesitan GitHub, instalar aplicaciones ni crear cuentas.

> GitHub guarda el código del proyecto, pero no ejecuta esta aplicación ASP.NET ni su base de datos. No hace falta publicar la web en Internet para usarla en casa.

### Preparación del portátil (solo la primera vez)

1. Instala el **SDK de .NET 10** desde [la página oficial de Microsoft](https://dotnet.microsoft.com/download/dotnet/10.0). El SDK sirve para iniciar la aplicación.
2. Descarga el proyecto desde GitHub con **Code → Download ZIP** y descomprime el archivo. Si ya tienes el proyecto en el portátil, salta este paso.
3. Abre la carpeta `HeroQuest` que contiene `HeroQuest.csproj`.
4. En esa carpeta, abre una terminal. En Windows puedes hacer clic derecho en un espacio vacío de la carpeta y elegir **Abrir en Terminal**.
5. Ejecuta este comando:

   ```powershell
   dotnet run --launch-profile lan
   ```

6. Si Windows pregunta si permite conexiones en el firewall, permite el acceso en **redes privadas**. Mantén el portátil despierto y la terminal abierta durante la partida.

La aplicación prepara la base de datos local al iniciarse. No hay que crearla manualmente.

### Abrirla desde cada teléfono

1. Conecta el móvil al mismo Wi‑Fi que el portátil. Evita la red Wi‑Fi de invitados, porque algunas redes impiden que los dispositivos se comuniquen entre sí.
2. En el portátil, abre **Configuración → Red e Internet → Wi‑Fi → Propiedades** y busca **Dirección IPv4**. También puedes escribir `ipconfig` en otra terminal y buscar la dirección IPv4 de la conexión Wi‑Fi.
3. En el navegador del móvil, abre `http://DIRECCION-IP:5080`, sustituyendo `DIRECCION-IP` por el número que aparece en el portátil. Por ejemplo: `http://192.168.1.34:5080`.

Todos los móviles conectados a ese Wi‑Fi pueden abrir la misma dirección. Cuando termine la partida, vuelve a la terminal del portátil y pulsa **Ctrl+C** para detener la web.

### Límites de esta modalidad

- Es gratuita y no necesita dominio ni alojamiento público.
- Solo funciona mientras el portátil esté encendido, la aplicación siga abierta y los móviles estén en la misma red local.
- La dirección IP puede cambiar al conectarse a otra red; basta con consultar la IPv4 de nuevo.
- Los amigos no tienen que instalar nada. Solo el portátil que sirve la web necesita el SDK de .NET 10.

## Desarrollo

El proyecto usa ASP.NET Core MVC, Entity Framework Core y SQLite. Para iniciarlo desde Visual Studio se puede usar el perfil `http`; para compartirlo en una red local, usa el perfil `lan` descrito arriba.
