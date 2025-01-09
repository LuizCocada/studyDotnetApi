namespace StudyAPI.Loggins;

public class CustomLoggerProviderConfiguration
{
     public LogLevel LogLevel { get; set; } = LogLevel.Warning;
     public int EventId { get; set; } = 0;
}

//loglevel define o nivel minimo de log a ser registrado; padrao é warning
//eventId define o id do evento a ser registrado; padrao é 0 