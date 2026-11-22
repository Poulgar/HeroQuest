# Guía visual de la web

La hoja de estilos principal ya no contiene cientos de reglas mezcladas. `wwwroot/css/site.css` funciona como un índice pequeño que carga, en orden, los módulos de `wwwroot/css/modules/`.

## Cómo se reparte el diseño

| Archivo | Qué controla |
| --- | --- |
| `base.css` | Colores originales de pergamino, tipografía, cabecera, navegación y pie. |
| `home.css` | Portada y paneles de acceso a las secciones. |
| `catalog.css` | Índices, listas, cartas de detalle y habilidades. |
| `map-and-dialogs.css` | Mapa, ventanas emergentes y textos de ayuda. |
| `forms-and-access.css` | Portal de acceso, emblemas, Biblioteca, formularios y botones fantásticos. |
| `themes.css` | Tema Luz de luna, con estética de grimorio mágico. |
| `responsive.css` | Cambios para tabletas y móviles. |

```text
site.css (índice)
  ├─ base → home → catálogos → mapa
  ├─ acceso y formularios → temas
  └─ responsive (se carga al final para ajustar los tamaños)
```

## Cambiar la apariencia

1. Para retocar una vista concreta, busca su módulo en la tabla. Por ejemplo, las tarjetas de monstruos están en `catalog.css`.
2. El botón de la barra superior alterna entre **Pergamino** y **Luz de luna**. El navegador recuerda la elección en ese dispositivo; no modifica datos del servidor.
3. Los colores comunes se declaran como variables CSS al principio de `base.css`. El tema alternativo reemplaza esas variables en `themes.css`.
4. Las reglas pequeñas de móvil se encuentran en `responsive.css`. Procura cambiar primero la regla existente del componente y añade una nueva solo si hace falta.
5. Mantén el orden de imports de `site.css`: las reglas responsive deben quedar al final para prevalecer en pantallas estrechas.

## Menú y cuentas de prueba

- `user / user123`: consulta el compendio sin herramientas de edición.
- `admin / admin123`: puede abrir **Biblioteca**, crear y editar fichas, eliminarlas o archivarlas.
- Archivar oculta la ficha de los catálogos públicos y permite restaurarla desde su Archivo de la Biblioteca. Eliminar sí la borra de SQLite.

Estas credenciales son solo de demostración local y no son adecuadas para publicar la web en Internet.

## Emblemas, botones y temas

La barra superior usa un emblema para abrir el portal: gris si no hay sesión, verde para el Aventurero y granate para el Maestro de Juego. El sello sol-luna es un único control que resalta el tema activo. Al cambiar de tema, el navegador solicita un verso en `WorldPoems` y lo muestra brevemente.

**Pergamino** mantiene la lectura de libro antiguo. **Luz de luna** cambia la escena a un grimorio: fondos azul violeta, tinta clara y destellos. Los botones usan bordes tallados y colores de cuero o magia; `responsive.css` conserva los ajustes para móvil.

La Biblioteca y el Archivo requieren la cuenta `admin`; la cuenta `user` se muestra como Aventurero y solo consulta los catálogos.
