## turnierplan.NET &middot; Documentation

This directory contains the turnierplan.NET documentation. The content files use an extended markdown format and are build into static HTML using [zensical](https://zensical.org). The documentation is hosted at [docs.turnierplan.net](https://docs.turnierplan.net).

In order to build the documentation locally, you must first install Python and `zensical`:

```
pip install -r requirements.txt
```

Next, you can either view the rendered documentation using the zensical-built-in server or you can generate the static website files:

```
python3 -m zensical serve  # starts a local web server on port 8000
python3 -m zensical build  # generates static web site artifacts into the 'site' directory
```
