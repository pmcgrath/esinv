pmcgrath @ October 2012


Have deleted the extracted samples directory due to issues with project warnings when the product is being build


Since we ignore all bin directories using an entry in the root .gitignore file
We have to cater for the bin sub directory within nunit
Since negation is so messy i have gone with option b in te link below from the list so i ran the following command
	git add -f lib/NUnit/bin/
See http://git.661346.n2.nabble.com/negated-list-in-gitignore-no-fun-td1675067.html
