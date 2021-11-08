#!bin/bash
docker pull ghcr.io/tkiserconsulting/pulse:edge
docker stop pulse
docker system prune -f
docker run -d --name=pulse ghcr.io/tkiserconsulting/pulse:edge