namespace SalesSheetConverter.Web.Spinner;
public interface ISpinnerProvider
{
    bool IsBusy { get; }
    void Wait();
    void Resume();
    Task WaitAndResumeAsync(Func<Task> action);
    Task<T> WaitAndResumeAsync<T>(Func<Task<T>> action);    
    event Action OnSpinnerStateChanged;
}

public class SpinnerProvider : ISpinnerProvider
{ 
    public bool IsBusy { get; private set; }
    public event Action? OnSpinnerStateChanged;

    public void Wait()
    {
        SetState(true);
    }
    
    public void Resume()
    {
       SetState(false);
    }

    public async Task WaitAndResumeAsync(Func<Task> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        Wait();
        await action();
        Resume();
    }
    
    public async Task<T> WaitAndResumeAsync<T>(Func<Task<T>> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        Wait();
        var resp = await action();
        Resume();
        return resp;
    }
    
    private void SetState(bool isBusy)
    {
        IsBusy = isBusy;
        //Fire this event to communicate the loading state to the parent layout,
        //allowing it to call StateHasChanged and update the display
        OnSpinnerStateChanged?.Invoke();
    }
   
}