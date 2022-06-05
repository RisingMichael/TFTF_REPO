# TFTF_REPO

Clone this repository onto a folder on you computer by typing into your git bash: 
"git clone <https-link>"

Open the git bash within the repository and make sure to install LFS to handle big files:
1. Download LFS from: https://git-lfs.github.com/
2. Install it by typing into your git bash
    "git lfs install"

If you want to work on the Unity Project than use Unity version 2022.1.3f1 since version conflicts could cause major headaches. 
To open the project just go to the unity hub and under open: "Add project from disk". Than select the project folder and the project starts.
![image](https://user-images.githubusercontent.com/75223967/172059868-2e9d9f23-70e3-4676-9b7e-fbbb434e8e92.png)

    
Install SourceTree for a better git-experience and to handle merges
  1. Download and install SourceTree: https://www.sourcetreeapp.com/
  2. Don't create an account at "Bitbucket" ( we don't need that)
  3. Add your local repository that you have cloned onto your computer to the source tree
  ![image](https://user-images.githubusercontent.com/75223967/172058881-7e701e19-2458-4c4f-a81c-9e8ff5a17682.png)
  4. At the menubar go to "Tools->Options->Diff": Change the Merge-Tool to Custom and copy into the Diff Command the path to your "UnityYAMLMerge.exe"-file. 
    The path should be something like this: ' C:\Program Files\Unity\Editor\Data\Tools\UnityYAMLMerge.exe '. 
    Than add to the Arguments tab the following: ' merge -p "$BASE" "$REMOTE" "$LOCAL" "$MERGED" '
  ![image](https://user-images.githubusercontent.com/75223967/172058908-4951c395-a6d8-47aa-a708-69202959f90d.png)
  5. From now try to use SourceTree to pull and push. 
  6. If a merge conflict comes up: In SourceTree right click on the file that causes the conflict -> Resolve Conflicts -> Launch External Merge Tool. 
      Now the conflicts should be merged automatically although redundancies may still occur!
  Here is a video on how to resolve merge conflicts using SourceTree: https://www.youtube.com/watch?v=yQvbaBgxA34&t=0s  
  
Things to make the git experience easier:
Within Unity: 
  1. Try not to work within the same scene (may not be avoidable)
  2. Don't change or even use prefabs from other scenes and/or workflows within Unity (for example UI-prefabs should be only used by the UI-people etc)
