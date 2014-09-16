$template = Get-Content .\template.html -Raw
$content = Get-Content .\index.md -Raw | Convert-Markdown
$data = $template.Replace("[Content]", $content);
Set-Content .\index.html $data