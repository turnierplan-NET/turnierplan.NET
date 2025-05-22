# Document API Load Tool

This script requests a PDF for a specific document in a loop to load the document API.

This script requires the following parameters:

- URL of the turnierplan.NET instance (default is local dev environment)
- Valid authentication token (can be acquired using browser dev tools)
- ID of an existing document
- Language code to request
- The number of processes which will run requests in parallel
- The number of requests each process will send out

> [!IMPORTANT]  
> The processes are not synchronized, so it is possible that one process can finish while another process has only done half its assigned requests.
