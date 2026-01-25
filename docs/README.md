## turnierplan.NET &middot; Documentation

This directory contains the source markdown files and mkdocs configuration for the publically available turnierplan.NET documentation: [https://docs.turnierplan.net](https://docs.turnierplan.net).

In order to build the documentation locally, you must first install Python and [mkdocs](https://www.mkdocs.org):

```
pip install -r requirements.txt
```

Next, you can either view the rendered documentation using the mkdocs-build-in server or you can generate the static website files:

```
python3 -m mkdocs serve  # starts a local web server on port 8000
python3 -m mkdocs build  # generates static web site artifacts into the 'site' directory
```
