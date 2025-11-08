import { ILogger, LogLevel } from '@microsoft/signalr';

/**
 * Custom logger for SignalR that integrates with browser console
 * and provides detailed debugging information for connection lifecycle
 */
export class SignalRLogger implements ILogger {
  private prefix = '[SignalR]';

  log(logLevel: LogLevel, message: string): void {
    const timestamp = new Date().toISOString();
    const formattedMessage = `${this.prefix} ${timestamp} - ${message}`;

    switch (logLevel) {
      case LogLevel.Critical:
      case LogLevel.Error:
        console.error(formattedMessage);
        break;
      case LogLevel.Warning:
        console.warn(formattedMessage);
        break;
      case LogLevel.Information:
        console.info(formattedMessage);
        break;
      case LogLevel.Debug:
      case LogLevel.Trace:
        console.debug(formattedMessage);
        break;
      default:
        console.log(formattedMessage);
    }
  }
}
