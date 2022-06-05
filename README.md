# FakeGit

## Requirements:
- .NET 6
- Web Development package from Visual Studio

## How to use FakeGit:

1. Upon opening the website, click on 'Click here to use FakeGit"
2. Provide the Path to the dictionary that you wish to log.
    - If you provide a relative path, it will start from the root of the project (for instance `~/data` will be `...\PUXinterviewMVC\~\data`).
3. After you prove the path, the website will validate the path. If the path exists, you will be redirected, otherwise the error will be displayed between the Generate button and the input field.
4. Upon being redirected, you can see the files that were changed

## How it works ?

- The FakeGit uses the hashes of the files in order to see whether there was a change.
- It uses MD5 hashes and stores them in the JSON in order to save the history.
- Because of this, we can overlap multiple direcotries. As the name suggest, the inspiration was taken from git's version monitoring.

## Known problem / implementation erros.

- Currently, this implementation is not handling erros properly. There are some checks and some exception throws, but in general, the errors are not logged, therefore it is hard to find what could cause an error if the website suddenly shuts down.
- Another very huge problem (and I would say the biggest) is that there are no tests present. Unfortunately I did not have time to make them yet (Each Service and Controller should be tested separately and controllers should be using mocked services in tests)
- UI of the app is just to check whether the app works. The UI needs to be completely redone (or more specifically remade)
- Currently, the app has services, yet some functionality from these services should have been moved to repository.
- `FileHandlingService::GetFiles` needs to be more polished (currently it is just 3 LINQ querries and a switch, but the functionality should be moved elsewhere, since this is not maintainable if more difficult logic needs to be implemented)
- `PathController::Check`, more specifically the switch in the enum iteration should be moved to a better place.
- The MD5 is for this purpose (in my opinion) enough, but from what I have heard a different hashing algorithm should be used.
- `FileHandlingService::CombineCachedAndCurrent` has a duplicity (2x almost the same forloop) that needs to be extracted 
- (a side one) -> I have not used git from the beginning and I am pushing directly to master. I know this is not the right way to develop.
- (nit pick) -> Forgot to add test file into `.gitignore`

