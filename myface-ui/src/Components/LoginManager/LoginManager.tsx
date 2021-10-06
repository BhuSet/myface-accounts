import React, {createContext, ReactNode, useState} from "react";
import { updateSpreadAssignment } from "typescript";

export const LoginContext = createContext({
    isLoggedIn: false,
    isAdmin: false,
    username: "",
    password: "",
    logIn: (username: string, password: string) => {},
    logOut: () => {}
});

interface LoginManagerProps {
    children: ReactNode
}

export function LoginManager(props: LoginManagerProps): JSX.Element {
    const [loggedIn, setLoggedIn] = useState(false);
    const [contextusername, setUsername] = useState("");
    const [contextPassword, setPassword] = useState("");
    
    function logIn(username: string, password: string) {
        setLoggedIn(true);
        setUsername(username);
        setPassword(password);
    }
    
    function logOut() {
        setLoggedIn(false);
        setUsername("");
        setPassword("");
    }
    
    const context = {
        isLoggedIn: loggedIn,
        isAdmin: loggedIn,
        username: contextusername,
        password: contextPassword,
        logIn: logIn,
        logOut: logOut 
    };

    console.log(`username = ${context.username}  password = ${context.password}`);
    
    return (
        <LoginContext.Provider value={context}>
            {props.children}
        </LoginContext.Provider>
    );
}