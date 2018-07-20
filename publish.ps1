$gitrepo = "https://github.com/Nivol/ToDoApi"
az webapp deployment source config -n test9087 -g test9087 --repo-url $gitrepo --branch master --manual-integration
az webapp deployment source sync -n test9087 -g test9087