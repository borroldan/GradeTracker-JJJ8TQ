using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Calculator.HTTP;

public class HttpServer : IDisposable
{
    private readonly TcpListener _listener;
    private readonly SemaphoreSlim _semaphore;
    private readonly ILogger _logger;
    private readonly int _port;
    private bool _isDisposed;

    public HttpServer(ILogger logger, int port = 8080)
    {
        if (port < 0 || port > ushort.MaxValue)
            throw new ArgumentOutOfRangeException($"{nameof(port)} was in invalid range");
        _logger = logger;
        _port = port;
        _listener = new TcpListener(IPAddress.Any, port);
        _semaphore = new SemaphoreSlim(10);
    }

    ~HttpServer()
    {
        Dispose(false);
    }

    protected virtual void Dispose(bool isDisposeCall)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        _listener.Dispose();
        _semaphore.Dispose();
        _isDisposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void Start()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        _listener.Start();
        _logger.Info("Server started");

    }

    public void Stop()
    {

    }
}
