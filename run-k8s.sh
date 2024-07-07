#!/usr/bin/bash

kubectl delete -R -f deployment/ 

kubectl apply -R -f deployment/