//*****************************************************************************
// This file is part of the At.Matus.Instruments.Gossen.Mavo project.
//
//
//*****************************************************************************

namespace At.Matus.Instruments.Gossen.Mavo
{
    public class ErrorHandler
    {
        public int ErrorNumber { get; private set; } = 0;
        public string ErrorCode { get; private set; } = "NO_ERROR";
        public string ErrorMessage { get; private set; } = string.Empty;
        public bool IsError => ErrorNumber != 0;
        public ErrorReaction OnError { get; set; } = ErrorReaction.DisplayMessage;
        public int TotalNumberOfQueries { get; private set; } = 0;
        public int NumberOfErrors { get; private set; } = 0;

        public void HandleError(int errorNumber)
        {
            TotalNumberOfQueries++;
            TranslateErrorCode(errorNumber);
            if (ErrorNumber != 0)
            {
                NumberOfErrors++;
                ReactToError();
            }
        }

        private void ReactToError()
        {
            string errorMessage = $"Error {ErrorNumber:D3}: {ErrorCode} - {ErrorMessage}";
            File.AppendAllText("error_log.txt", errorMessage + Environment.NewLine);
            switch (OnError)
            {
                case ErrorReaction.Ignore:
                    break;
                case ErrorReaction.ThrowException:
                    throw new InvalidOperationException(errorMessage);
                case ErrorReaction.DisplayMessage:
                    Console.WriteLine(errorMessage);
                    break;
            }
        }

        private void TranslateErrorCode(int errorNumber)
        {
            ResetError();
            ErrorNumber = errorNumber;
            switch (errorNumber)
            {
                case 0:
                    break;
                case 1:
                    ErrorCode = "UART_ERR_OVE";
                    ErrorMessage = "Overrun error: A new character was read in before the current character was picked up.";
                    break;
                case 2:
                    ErrorCode = "UART_ERR_FE";
                    ErrorMessage = "Transmission error: stop bit not detected";
                    break;
                case 3:
                    ErrorCode = "UART_ERR_PE";
                    ErrorMessage = "Transmission error: parity error";
                    break;
                case 4:
                    ErrorCode = "UART_ERR_BUFFOFL";
                    ErrorMessage = "Receive buffer overrun";
                    break;
                case 8:
                    ErrorCode = "UART_ERR_TIMEOUT";
                    ErrorMessage = "Timeout, no frame end detected";
                    break;
                case 17:
                    ErrorCode = "ADC_ERR_PHASE";
                    ErrorMessage = "ADW Phase sequence not adhered to";
                    break;
                case 18:
                    ErrorCode = "ADC_ERR_OFL";
                    ErrorMessage = "Timeout during deintegration (= overflow)";
                    break;
                case 19:
                    ErrorCode = "ADC_ERR_OVR";
                    ErrorMessage = "Measuring range exceeded (= overrange)";
                    break;
                case 21:
                    ErrorCode = "EEP_ERR_WRITE";
                    ErrorMessage = "EEPROM Write error";
                    break;
                case 22:
                    ErrorCode = "EEP_ERR_LOCKED";
                    ErrorMessage = "Impermissible access to calibration data memory";
                    break;
                case 101:
                    ErrorCode = "SCPI_ERR__CMD_NOT_FOUND";
                    ErrorMessage = "Error in header, command not supported";
                    break;
                case 102:
                    ErrorCode = "SCPI_ERR__WRONG_PARA_COUNT";
                    ErrorMessage = "Wrong number of parameters";
                    break;
                case 103:
                    ErrorCode = "SCPI_ERR__WRONG_PARA_TYPE";
                    ErrorMessage = "Unexpected parameter type";
                    break;
                case 104:
                    ErrorCode = "SCPI_ERR__WRONG_PARA_UNITS";
                    ErrorMessage = "Incorrect unit of measure for parameter";
                    break;
                case 105:
                    ErrorCode = "SCPI_ERR__UNMATCHED_QUERY";
                    ErrorMessage = "Query command not implemented";
                    break;
                case 106:
                    ErrorCode = "SCPI_ERR__UNMATCHED_BRACKET";
                    ErrorMessage = "Bracket error: The number of opening and closing brackets is not identical.";
                    break;
                case 107:
                    ErrorCode = "SCPI_ERR__INVALID_VALUE_LIST";
                    ErrorMessage = "Setting value outside of permissible range";
                    break;
                case 108:
                    ErrorCode = "SCPI_ERR__INVALID_NUM_SUFFIX";
                    ErrorMessage = "Incorrect index value";
                    break;
                case 201:
                    ErrorCode = "SCPI_ERR__DEVICE_UNKNOWN";
                    ErrorMessage = "Hardware is not supported by this firmware version";
                    break;
                case 202:
                    ErrorCode = "SCPI_ERR__WRONG_SENSOR";
                    ErrorMessage = "The connected sensor is not suitable for the requested measurement.";
                    break;
                case 203:
                    ErrorCode = "SCPI_ERR__RANGE_OVR";
                    ErrorMessage = "Measuring Range exceeded";
                    break;
                case 204:
                    ErrorCode = "SCPI_ERR__WRONG_PASSWORD";
                    ErrorMessage = "Incorrect password";
                    break;
                default:
                    ErrorCode = "UNKNOWN_ERROR";
                    ErrorMessage = "Unknown error";
                    break;
            }
        }

        private void ResetError()
        {
            ErrorNumber = 0;
            ErrorCode = "NO_ERROR";
            ErrorMessage = string.Empty;
        }
    }

    public enum ErrorReaction
    {
        Ignore,
        ThrowException,
        DisplayMessage
    }

}
