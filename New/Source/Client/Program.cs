using FinalFrontier.Core;

#if RELEASE
try
{
#endif
    using var game = new Game();
    game.Run();
#if RELEASE
}
catch (Exception ex)
{
    ElementEngine.Logging.Error(ex.ToString());
    ElementEngine.Logging.Dispose();
}
#endif