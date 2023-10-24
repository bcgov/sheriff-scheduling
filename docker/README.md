# Running the Application on Docker

## Management Script

The `manage` script wraps the Docker process in easy to use commands.

To get full usage information on the script, run:
```
./manage -h
```
  
## Building the Images

The first thing you'll need to do is build the Docker images. 

To build the images, run:
```
./manage build
```

## Restore a backup of the database

Create a `/c/court-admin-scheduling/docker/tmp` folder if it does not exist.
Place a backup archive in that folder.

### Using the `restore` command (Recommended)

This is the recommended approach to restore a backup in order to initialize/reset the API.  The `restore` command automates the process of resetting the API's database and restoring a fresh copy from backup, allowing you to easily initialize or reset the API during development and testing.
```
Wade@Epoch MINGW64 /c/court-admin-scheduling/docker (master)
$ ./manage restore postgres-appdb_2020-03-06_13-42-56.sql.gz
```

## Starting the Project

To start the project, run:
```
./manage start
```

This will start the project interactively; with all of the logs being written to the command line.  Press `Ctrl-C` to shut down the services from the same shell window.

## Stopping the Project

To stop the project, run:
```
./manage stop
```

This will shut down and clean up all of the containers in the project.  This is a non-destructive process.  The containers are not deleted so they will be reused the next time you run start.

Since the services are started interactively, you will have to issue this command from another shell window.  This command can also be run after shutting down the services using the `Ctrl-C` method to clean up any services that may not have shutdown correctly.

## Using the Application

- The main UI is exposed at; http://localhost:8080/
- The API is available at; http://localhost:8080/api
  - The API is also exposed directly at; http://localhost:5000/api