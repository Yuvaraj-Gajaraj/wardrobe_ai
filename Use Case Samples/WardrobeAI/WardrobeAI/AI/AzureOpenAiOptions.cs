namespace WardrobeAI.AI;

/// <summary>
/// Strongly-typed binding for the <c>AzureOpenAI</c> configuration section
/// (see <c>appsettings.json</c> + <c>Preferences</c> overrides).
///
/// <para>All three values are intentionally empty strings until the user
/// supplies their Azure OpenAI endpoint, key, and deployment name. The
/// service short-circuits with a friendly
/// <c>Result.Fail("AI.NotConfigured", "...")</c> when the endpoint or
/// deployment is blank — the dashboard falls back to its existing static
/// placeholder copy instead of throwing at the user.</para>
/// </summary>
public sealed class AzureOpenAiOptions
{
    /// <summary>
    /// e.g. <c>https://your-resource.openai.azure.com/</c> (trailing slash required
    /// by the Azure OpenAI REST contract).
    /// </summary>
    public string Endpoint { get; set; } = "END_POINT";

    /// <summary>Azure OpenAI API key. Prefer Managed Identity in production; this is the dev-only path.</summary>
    public string ApiKey { get; set; } = "API_KEY";

    /// <summary>Deployment name (a.k.a. "model" in the Azure portal — e.g. <c>gpt-4o</c>).</summary>
    public string Deployment { get; set; } = "MODEL";

    /// <summary>API version. Defaults to a stable GA version known to support JSON-schema structured outputs.</summary>
    public string ApiVersion { get; set; } = "API_VERSION";

    /// <summary>Sampling temperature. 0.3 keeps the outfit picker stable + reproducible for the same closet.</summary>
    public double Temperature { get; set; } = 0.3;

    /// <summary>Max completion tokens. The structured-output payload is small (~600 tokens) so 800 is plenty.</summary>
    public int MaxOutputTokens { get; set; } = 800;

    /// <summary>
    /// True when the user has wired Azure OpenAI in. Drives the friendly
    /// fallback path.
    /// </summary>
    /// <remarks>
    /// The endpoint must parse as an absolute HTTP/HTTPS URI — the
    /// placeholder defaults (<c>"END_POINT"</c>, <c>"API_KEY"</c>,
    /// <c>"MODEL"</c>) are non-empty but not valid URIs, and previously
    /// the Azure OpenAI services' singleton ctors would eagerly call
    /// <c>new AzureOpenAIClient(new Uri(opts.Endpoint), ...)</c>, throwing
    /// <c>UriFormatException: Invalid URI: The format of the URI could not
    /// be determined.</c> at app startup before any fallback path could
    /// run. Validating the URI here keeps the singletons constructable
    /// against placeholder config so the deterministic offline path can
    /// take over without crashing the app on first run.
    /// </remarks>
    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(Endpoint)
        && !string.IsNullOrWhiteSpace(ApiKey)
        && !string.IsNullOrWhiteSpace(Deployment)
        && Uri.TryCreate(Endpoint, UriKind.Absolute, out var endpointUri)
        && (endpointUri.Scheme == Uri.UriSchemeHttp || endpointUri.Scheme == Uri.UriSchemeHttps);
}
