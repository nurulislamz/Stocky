using stockymodels.Events;
using stockymodels.models;

namespace stockyapi.CommandToEventHandler;

/// <summary>
/// All changes for one command: domain events, persistence event models, and updated projection.
/// Built by the pipeline (CommandToEvents → ToEventModels, UpdateProjection) then executed in one transaction or turned into SQL.
/// </summary>
