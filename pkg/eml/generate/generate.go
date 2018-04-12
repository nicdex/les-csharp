package generate

import (
	"path/filepath"
	"os"
	"github.com/nicdex/les-csharp/pkg/eml/generate/csharp"
	"github.com/Adaptech/les/pkg/eml"
)

const csharpExt = ".cs"

func Generate(system eml.Solution, renderingDirectory string, infrastructureTemplateDirectory string) {
	var csharpRenderingDirectory = renderingDirectory
	var csharpTemplateDirectory = filepath.Join(infrastructureTemplateDirectory, "csharp")
	var eventsDirectory = filepath.Join(csharpRenderingDirectory, "src", "Les.CSharp.Domain", "Events")
	var commandsDirectory = filepath.Join(csharpRenderingDirectory, "src", "Les.CSharp.Domain", "Commands")
	var domainDirectory = filepath.Join(csharpRenderingDirectory, "src", "Les.CSharp.Domain", "Domain")
	var controllerDirectory = filepath.Join(csharpRenderingDirectory, "src", "Les.CSharp.Web", "Controllers")
	var readmodelDirectory = filepath.Join(csharpRenderingDirectory, "src", "Les.CSharp.Domain", "ReadModels")

	deleteAllExceptNodeModules(csharpRenderingDirectory)
	copyInfrastructureTemplate(csharpTemplateDirectory, csharpRenderingDirectory)

	// C# considerations
	// - add files in .csproj

	for _, boundedContext := range system.Contexts {
		for _, stream := range boundedContext.Streams {
			for _, event := range stream.Events {
				renderedJavascript := csharp.EventToJs(event)
				writeRenderedEvent(eventsDirectory, csharp.ToCSharpClassName(event.Event.Name), renderedJavascript, csharpExt)
			}
			for _, command := range stream.Commands {
				renderedJavascript := csharp.CommandToJs(command)
				writeRenderedCommand(commandsDirectory, csharp.ToCSharpClassName(command.Command.Name), renderedJavascript, csharpExt)
			}
			renderedJavaScript := csharp.DomainJs(stream, stream.Events)
			writeRenderedAggregate(domainDirectory, stream.Name, renderedJavaScript, csharpExt)

			renderedJavaScript = csharp.ControllerJs(stream, readmodelLookupFor(boundedContext))
			writeRenderedController(controllerDirectory, stream.Name, renderedJavaScript, csharpExt)

		}
		eventLookup := make(map[string]eml.Event)
		for _, stream := range boundedContext.Streams {
			for _, event := range stream.Events {
				eventLookup[event.Event.Name] = event
			}
		}
		for _, readmodel := range boundedContext.Readmodels {
			readModelJs := csharp.ReadmodelsToJs(readmodel, eventLookup)
			writeRenderedReadmodel(readmodelDirectory, csharp.ToCSharpClassName(readmodel.Readmodel.Name), readModelJs, csharpExt)
		}
	}
}


func deleteAllExceptNodeModules(nodeJsRenderingDirectory string) {
	os.RemoveAll(nodeJsRenderingDirectory)
	/*
	files, err := filepath.Glob(filepath.Join(nodeJsRenderingDirectory, "*.*"))
	if err != nil {
		panic(err)
	}
	for _, f := range files {
		if err := os.RemoveAll(f); err != nil {
			panic(err)
		}
	}
	if err := os.RemoveAll(filepath.Join(nodeJsRenderingDirectory, "Dockerfile")); err != nil {
		log.Fatalf("NodeAPI deletePreviouslyRendered failed: %v", err)
	}
	if err := os.RemoveAll(filepath.Join(nodeJsRenderingDirectory, "config")); err != nil {
		log.Fatalf("NodeAPI deletePreviouslyRendered failed: %v", err)
	}
	if err := os.RemoveAll(filepath.Join(nodeJsRenderingDirectory, "Les.CSharp.Domain")); err != nil {
		log.Fatalf("NodeAPI deletePreviouslyRendered failed: %v", err)
	}
	if err := os.RemoveAll(filepath.Join(nodeJsRenderingDirectory, "Les.CSharp.Tests")); err != nil {
		log.Fatalf("NodeAPI deletePreviouslyRendered failed: %v", err)
	}
	if err := os.RemoveAll(filepath.Join(nodeJsRenderingDirectory, "Les.CSharp.Web/Controllers")); err != nil {
		log.Fatalf("NodeAPI deletePreviouslyRendered failed: %v", err)
	}
	*/
}

func readmodelLookupFor(boundedContext eml.BoundedContext) map[string]eml.Readmodel {
	readmodelLookup := make(map[string]eml.Readmodel)
	for _, readmodel := range boundedContext.Readmodels {
		readmodelLookup[readmodel.Readmodel.Name] = readmodel
	}
	return readmodelLookup
}
