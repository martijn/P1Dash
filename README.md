# P1 Dash

![Screen shot showing a guage based on P1 data](screenshot.gif)

A simple web interface for the Dutch Smart Meter reader (DSMR/ESMR v5).
Right now it does only one thing; display a guage to show you the
power consumed from or fed back to the grid in real time.

## Run with Docker

P1 Dash can connect either through a local serial port (using a suitable FTDI
serial cable) or to a TCP socket served by ser2net or a dedicated smart meter
interface.

Build and run P1 Dash by cloning the repostory and executing the following commands:

```
git clone https://github.com/martijn/P1Dash.git
cd P1Dash

docker build -t p1dash .
docker run -d --name p1dash -p 5000:5000 --restart=always -v p1dash-storage:/app/Storage --device /dev/ttyUSB0 p1dash
```

If you intend to use a TCP socket you can omit the `--device /dev/ttyUSB0` part.

After startup, visit `http://localhost:5000` and click the cog icon to access the settings.
dialog in the applciation.