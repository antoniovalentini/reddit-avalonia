namespace Reddit.Client;

public class RedditClientSettings
{
    public WebAuthParameters WebAuthParameters { get; set; }
    public RedditClientSettings(WebAuthParameters webAuthParameters)
    {
        WebAuthParameters = webAuthParameters;
    }

    // TODO: fix Cannot dynamically create an instance of type 'Reddit.Core.RedditClientSettings'. Reason: No parameterless constructor defined. and remove this empty constructor
    public RedditClientSettings()
    {
        WebAuthParameters = new WebAuthParameters(0, "", "", "", "");
    }
};
public record WebAuthParameters(int Port, string Host, string AppId, string AppSecret, string RelativeRedirectUri);
