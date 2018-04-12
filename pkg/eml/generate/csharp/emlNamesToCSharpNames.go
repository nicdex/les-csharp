package csharp

import "strings"

// ToCSharpClassName ...
func ToCSharpClassName(s string) string {
	return strings.Replace(s, " ", "", -1)
}
